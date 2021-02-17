﻿using BL.BLApi;
using BLApi;
using BO;
using DLApi;
using PriorityQueue;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BL.simulator
{
    class TravelsExecuter
    {
        #region singelton

        TravelsExecuter() 
        {
            source = BLFactory.GetBL("admin");//get a BL instance to load data from the dal(its more convenient then to use directly with dal)
        }
        static TravelsExecuter(){}
        static TravelsExecuter instance;
        static public TravelsExecuter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TravelsExecuter();
                }
                return instance;
            }
        }

        #endregion

        /// <summary>
        /// <br>this fild is save how much time before the start</br>
        /// <br>time of the travel the travel executer will lunch the</br>
        /// <br>backgrownd worker of this travel</br>
        /// <br>(in simulator time)</br>
        /// </summary>
        TimeSpan timeToExcuteTravel = new TimeSpan(0, 20, 0);

        Action<LineTiming> observer;
        volatile List<int> stationsInTrack = new List<int>();//list of all the station that in tracking

        IBL source;

        RidesSchedule schedule = RidesSchedule.Instance;

        SimulationClock clock;

        Thread travelsExecuterThread;
        public void StartExecute(Action<LineTiming> _observer = null)
        {
            if(source == null)
            {
                source = BLFactory.GetBL("admin");
            }

            clock = SimulationClock.Instance;
            observer = _observer != null ? _observer : (LineTiming) => { };//if _observer is null then set observer to be an Action<LineTiming> that do nothing

            //load the lineTrips and lines
            var lineTrips = source.GetAllLineTrips();

            //build the schedual
            schedule.Set_lineTrips(lineTrips);
            if(travelsExecuterThread != null)
            {
                travelsExecuterThread.Interrupt();
                travelsExecuterThread.Join();//whit for the travelsExecuterThread to finish his current tasks
            }
            travelsExecuterThread = new Thread((object schedual) => execute_rides(schedual, travelsExecuterThread));//preform the func execute_rides with 'schdual' and the travel xecuter thread
            travelsExecuterThread.Name = "rides executer";//set the name of the thread to be "rides executer" for maintenance
            travelsExecuterThread.Start(schedule);//start the travelsExecuterThread with argument 'schedule'
        }

        private void executeTravel(Ride ride)
        {
            BackgroundWorker newTravel = new BackgroundWorker();
            newTravel.DoWork += (object sender, DoWorkEventArgs args) =>
            {
                //get the line of this line trip
                var temp = TimeSpan.Zero;
                Stopwatch st = new Stopwatch();
                Line line = source.GetLine(ride.LineId);

                string lastStationName = source.GetStation(line.LastStation.StationNumber).Name;//get the name of the last station in this line
                int lineNumber = line.LineNumber;

                LineStation[] stations = line.Stations.ToArray();
                Dictionary<int, LineTiming> underTruck = new Dictionary<int, LineTiming>();//save the code of all the stations in this line that under truck together with the apropiate lineTiming  

                #region wait to start
                TimeSpan timeUntilStart = ride.StartTime - clock.Time;
                waite_while_doing(clock.Rtime_to_Stime(timeUntilStart), () =>
                {
                    foreach (int station in stationsInTrack)//update the observer for all the stations that under truck
                    {
                        LineStation tempLS = line.Stations.FirstOrDefault(ls => ls.StationNumber == station);//get the line station of this station in this line
                        if (tempLS == null)//if tempLS is null then this station not exist in this line
                        {
                            continue;
                        }
                        TimeSpan arrivalTime = tempLS.Time_from_start + ride.StartTime - clock.Time;//calculate the arrival time for this station
                        LineTiming timing = new LineTiming()
                        {
                            LineNum = line.LineNumber,
                            LineId = line.ID,
                            LastStation = lastStationName,
                            StartTime = ride.StartTime,
                            StationCode = station,
                            ArrivalTime = arrivalTime
                        };
                        if(!clock.Cancel)
                            observer(timing);//update the observer
                    }
                    return clock.Cancel;
                }); 
                #endregion
                for (int current = 0; current < stations.Length - 1 && !clock.Cancel; current++)//the loop to not get to the last station because in the last station it's irelvant
                {
                    #region betwen stations 
                    //calculate the sleep time (the time until the next station)
                    long currentToNext_time = (stations[current].CurrentToNext != null ? (long)stations[current].CurrentToNext.Time.TotalMilliseconds : 0L);//if its the last station in the line then the 'CurrentToNext' fild will be null
                    //currentToNext_time *= (new Random()).Next(10, 200) / 100;//real time is between 10% and 200% of the avrege time(the time that save in the LineStation)
                    long sleepTimeMiliSeconds = currentToNext_time;//in real world time
                    TimeSpan sleepTime = new TimeSpan(0, 0, 0, 0, (int)sleepTimeMiliSeconds);//in real world time
                    int updateRate = 1000;//the rate that the observer will be update while waiting to arrive to the next station(in miliseconds)
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Restart();
                    //while the bus is between stations update the observer any 1 second about the arrival time
                    while(stopwatch.Elapsed < clock.Rtime_to_Stime(sleepTime))
                    {
                        double sleep = Math.Min(sleepTimeMiliSeconds - stopwatch.ElapsedMilliseconds, updateRate);
                        Thread.Sleep((int)sleep);

                        if(clock.Cancel)//this part can take a lot of time so check any second the cancel state
                        {
                            break;
                        }

                        foreach (int station in stationsInTrack)//update the observer for all the stations that under truck
                        {
                            LineStation tempLS = line.Stations.FirstOrDefault(ls => ls.StationNumber == station);
                            LineStation nearStation = line.Stations[current + 1];
                            if (tempLS == null || tempLS.LineStationIndex < nearStation.LineStationIndex)//if this station not in this line or the bus allready pass this station
                            {
                                continue;
                            }
                            TimeSpan arrivalTime = max(TimeSpan.Zero, tempLS.Time_from_start - nearStation.Time_from_start + sleepTime - clock.Stime_to_Rtime(stopwatch.Elapsed));//if the time appens to be less then 0:0:0 then set arrivalTime to be 0:0:0
                            LineTiming timing = new LineTiming()
                            {
                                LineNum = line.LineNumber,
                                LineId = line.ID,
                                LastStation = lastStationName,
                                StartTime = ride.StartTime,
                                StationCode = station,
                                ArrivalTime = arrivalTime
                            };
                            if(!clock.Cancel)
                                observer(timing);//update the observer 
                        }
                    }
                    stopwatch.Stop();
                    #endregion
                }
            };
            newTravel.RunWorkerAsync();
        }

        private int Get_bus_for_ride()
        {
            throw new NotImplementedException("Get_bus_for_ride()");
        }

        /// <summary>
        /// add the stationCode to the list of the stations under truck
        /// </summary>
        public void Add_station_to_truck(int stationCode)
        {
            stationsInTrack.Add(stationCode);
        }

        /// <summary>
        /// removes the stationCode from the list of the station under truck
        /// </summary>
        public void Stop_truck_station(int stationCode)
        {
            stationsInTrack.Remove(stationCode);
        }

        /// <summary>
        /// return the time in the day that 'duretionBack' before 'pivotTime'
        /// </summary>
        private TimeSpan time_back(TimeSpan pivotTime, TimeSpan duretionBack)
        {
            TimeSpan result = pivotTime - duretionBack;
            while(result < TimeSpan.Zero)
            {
                result += new TimeSpan(1, 0, 0, 0);
            }
            return result;
        }

        /// <summary>
        /// returns the duration from 'pivotTime' to 'dstTime' as times in the day
        /// </summary>
        private TimeSpan timeUntil(TimeSpan pivotTime, TimeSpan dstTime)
        {
            TimeSpan result = dstTime - pivotTime;
            while (result < TimeSpan.Zero)
            {
                result += new TimeSpan(1, 0, 0);
            }
            return result;
        }

        /// <summary>
        /// return the bigest value acorrding to ComperTo()
        /// </summary>
        private T max<T>(params T[] values) where T : IComparable
        {
            T max_val = values[0];
            foreach(T value in values)
            {
                max_val = max_val.CompareTo(value) >= 0 ? max_val : value;
            }
            return max_val;
        }

        /// <summary>
        /// return the smallest value acorrding to ComperTo()
        /// </summary>
        private T min<T>(params T[] values) where T : IComparable
        {
            T min_val = values[0];
            foreach (T value in values)
            {
                min_val = min_val.CompareTo(value) <= 0 ? min_val : value;
            }
            return min_val;
        }

        #region functions for threads
        /// <summary>
        /// executs rides according to the rides schedual
        /// </summary>
        /// <param name="obj_schedule">the schedual of the rides to execute</param>
        /// <param name="thisThread">the thrad that this execution of the function is runing on</param>
        private void execute_rides(object obj_schedule, Thread thisThread)
        {
            if (!(obj_schedule is RidesSchedule schedual))
            {
                throw new IligalArgsPassedToFunction("execute_rides(): paramete has to be RidesSchedule type");
            }
            while (!clock.Cancel)
            {
                TimeSpan sleep = schedule.time_until_next_ride();//get the time until the next ride from the schedule(in simulator time)
                sleep = clock.Stime_to_Rtime(sleep);//convert 'sleep' to real world time to the next ride
                try
                {
                    Thread.Sleep(sleep);//sleep 'sleep' time
                }
                catch (ThreadInterruptedException)//if the "travel executer thread" whas interrupted while sleeping
                {
                    break;//end the loop
                }               
                executeTravel(schedule.GetNextRide());//execute the ride
            }
        }


        /// <summary>
        /// do 'action' every 'sleepTime' miliseconds for 'time' time
        /// </summary>
        /// <param name="action">if returns true the function will be end imidietly</param>
        private void waite_while_doing(TimeSpan time, Func<bool> action, int sleepTime = 1000)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while(stopwatch.Elapsed < time)
            {
                bool isInterrupted = action();
                if(isInterrupted)
                {
                    break;
                }
                int sleep = (int)Math.Min(sleepTime, time.TotalMilliseconds - stopwatch.ElapsedMilliseconds);
                if(sleep < 0)
                {
                    break;
                }
                Thread.Sleep(sleep);
            }
            stopwatch.Stop();
        }

        #endregion
    }
}
