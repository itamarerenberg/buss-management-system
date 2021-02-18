﻿using BL.BLApi;
using BLApi;
using BO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BL.simulator
{
    public class Garage
    {
        #region singelton

        Garage() { }
        static Garage() { }
        static Garage instance;
        static public Garage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Garage();
                }
                return instance;
            }
        }

        #endregion

        /// <summary>
        /// the time it's take to treat a bus
        /// </summary>
        private readonly TimeSpan Treatment_time = new TimeSpan(0, 30, 0);//30 minuts

        /// <summary>
        /// the time it's take to refule a bus
        /// </summary>
        private readonly TimeSpan Refule_time = new TimeSpan(0, 5, 0);//5 minuts

        /// <summary>
        /// the amount of fule that the bus will have after refule
        /// </summary>
        private readonly int Max_fule_in_bus = 1200;

        public Action<BusProgress> Observer { get; set; } = (p) => { };

        /// <summary>
        /// lunch a thread that simulate a refuling prosess
        /// </summary>
        /// <param name="bus">the bus to refule</param>
        public void Refule(Bus bus)
        {
            Thread refuler = new Thread(() =>
            {
                SimulationClock clock = SimulationClock.Instance;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                IBL source = BLFactory.GetBL("admin");
                bus.Stat = BusStatus.In_treatment;
                source.UpdateBus(bus);//update the data source that the bus is in refuling now
                while (!clock.Cancel && stopwatch.Elapsed < Refule_time)
                {
                    try
                    {
                        //update observer
                        BusProgress progress = new BusProgress()
                        {
                            BusLicensNum = bus.LicenseNumber,
                            Activity = Activities.Refuling,
                            Progress = (float)(100 * stopwatch.Elapsed.TotalMilliseconds / Refule_time.TotalMilliseconds)//the presentege of the refule that pass allready
                        };
                        Observer(progress);

                        int sleep = (int)Math.Min((int)clock.Stime_to_Rtime(1000), (Refule_time - stopwatch.Elapsed).TotalMilliseconds);//the minimum between 1 second and the time that rimeins to the refuling prosess
                        sleep = Math.Max(sleep, 0);//if sleep turn out to be less then zero so set sleep to 0
                        Thread.Sleep((int)clock.Stime_to_Rtime(1000));
                    }
                    catch (ThreadInterruptedException)
                    {
                        return;
                    }
                }
                //update the bus Fule state
                if (!clock.Cancel)
                {
                    //update observer
                    BusProgress progress = new BusProgress()
                    {
                        BusLicensNum = bus.LicenseNumber,
                        Activity = Activities.Refuling,
                        Progress = 100//update the observer that the refuling proses has been finished
                    };
                    Observer(progress);
                    bus.Fuel = Max_fule_in_bus;
                    //set the bus's new status
                    bus.Stat = bus.KmAfterTreat >= Bus.max_km_without_tratment
                    || bus.LastTreatDate < DateTime.Now - Bus.max_time_without_tratment 
                    ? BusStatus.Need_treatment//if the bus need treatment
                    : BusStatus.Ready;//else

                    source.UpdateBus(bus);
                }
            });
            refuler.Name = "refuler " + bus.LicenseNumber;
            refuler.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        public void Treatnent(Bus bus)
        {
            Thread treatment = new Thread(() => 
            {
                SimulationClock clock = SimulationClock.Instance;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                IBL source = BLFactory.GetBL("admin");
                bus.Stat = BusStatus.In_treatment;
                source.UpdateBus(bus);//update the data source that the bus is in treatment now
                while (!clock.Cancel && stopwatch.Elapsed < Refule_time)
                {
                    try
                    {
                        //update observer
                        BusProgress progress = new BusProgress()
                        {
                            BusLicensNum = bus.LicenseNumber,
                            Activity = Activities.InTrartment,
                            Progress = (float)(100 * stopwatch.Elapsed.TotalMilliseconds / Treatment_time.TotalMilliseconds)//the presentege of the treatment that pass allready
                        };
                        Observer(progress);

                        int sleep = (int)Math.Min((int)clock.Stime_to_Rtime(1000), (Refule_time - stopwatch.Elapsed).TotalMilliseconds);//the minimum between 1 second and the time that rimeins to the refuling prosess
                        sleep = Math.Max(sleep, 0);//if sleep turn out to be less then zero so set sleep to 0
                        Thread.Sleep((int)clock.Stime_to_Rtime(1000));
                    }
                    catch (ThreadInterruptedException)
                    {
                        return;
                    }
                }
                //update the bus Fule state
                if (!clock.Cancel)
                {
                    //update observer
                    BusProgress progress = new BusProgress()
                    {
                        BusLicensNum = bus.LicenseNumber,
                        Activity = Activities.Refuling,
                        Progress = 100//update the observer that the refuling proses has been finished
                    };
                    Observer(progress);
                    bus.KmAfterTreat = 0;
                    bus.LastTreatDate = DateTime.Now - DateTime.Now.TimeOfDay + clock.Time;//the current real world date with the time in the dey = clock.Time
                    bus.Stat = bus.Fuel >= Bus.min_fule_befor_warning ? BusStatus.Ready : BusStatus.Need_refueling;
                    source.UpdateBus(bus);
                }
            });
            treatment.Name = "treatment " + bus.LicenseNumber;
            treatment.Start();
        }
    }
}