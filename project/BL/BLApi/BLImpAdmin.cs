﻿using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.BLApi;
using BL.simulator;
using BLApi;
using BO;
using DLApi;

namespace BL
{
    public class BLImpAdmin : IBL
    {
        #region singelton

        static readonly BLImpAdmin instance = new BLImpAdmin();
        static BLImpAdmin() { }
        BLImpAdmin() { }
        public static BLImpAdmin Instance { get => instance; }

        #endregion

        IDL dl = DLFactory.GetDL();
        Garage garage = Garage.Instance;
        #region Manager
        public bool GetManagar(string name, string password)
        {
            try
            {
                //validation
                DO.User user = dl.GetUser(name);

                if (user.Admin == false)
                    throw new InvalidInput("the user doesn't have an administrator access");

                if (user.Password != password)
                    throw new InvalidPassword("invalid password");

                return true;
            }
            catch (DO.InvalidObjectExeption)
            {
                throw new InvalidID("invalid name");
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        public void AddManagar(string name, string password)
        {
            try
            {
                DO.User tempUser = new DO.User()
                {
                    Name = name,
                    Password = password,
                    Admin = true,
                    IsActive = true
                };
                dl.AddUser(tempUser);
            }
            catch (DO.DuplicateExeption msg)
            {
                throw msg;
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        public void UpdateManagar(string oldName, string oldPassword, string newName, string newPassword)
        {
            try
            {
                DO.User oldManagar = dl.GetUser(oldName);
                if (oldManagar.Password != oldPassword)
                    throw new InvalidInput("the old password is incorect");
                if (oldManagar.Admin == false)
                    throw new InvalidInput("the user doesn't have an administrator access");
                DO.User newManagar = new DO.User()
                {
                    Name = newName,
                    Password = newPassword,
                    Admin = true,
                };
                dl.UpdateUser(newManagar);
            }
            catch (DO.NotExistExeption)
            {
                throw new InvalidPassword("the password or name are invalid");
            }
            catch (InvalidInput msg)
            {
                throw msg;
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        public void DeleteManagar(string name, string password)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Bus

        public void AddBus(Bus bus)
        {
            //check if the langth of the LicenseNum fit to the LicensDate
            if (bus.LicenesDate.Year >= 2018)
            {
                if (bus.LicenseNumber.Length != 8)
                {
                    throw new InvalidInput("Licens number is not fit to the licens date");
                }
            }
            else
            {
                if (bus.LicenseNumber.Length != 7)
                {
                    throw new InvalidInput("Licens number is not fit to the licens date");
                }
            }
            try
            {
                dl.AddBus((DO.Bus)bus.CopyPropertiesToNew(typeof(DO.Bus)));
            }
            catch (Exception msg)
            {
                throw msg;
            }

        }
        public Bus GetBus(string licensNum)
        {
            try
            {
                Bus bus = (Bus)dl.GetBus(licensNum).CopyPropertiesToNew(typeof(Bus));// get do bus
                bus.BusTrips = dl.GetAllBusTripsBy(b => b.Bus_Id == licensNum).Select(bt => (BusTrip)bt.CopyPropertiesToNew(typeof(BusTrip))).ToList();// get list of bo bus trip
                return bus;
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        public void UpdateBus(Bus bus)
        {
            try
            {
                DO.Bus tempBus = (DO.Bus)bus.CopyPropertiesToNew(typeof(DO.Bus));
                dl.UpdateBus(tempBus);
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        public void DeleteBus(string licensNum)
        {
            try
            {
                dl.DeleteBus(licensNum);
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        public IEnumerable<Bus> GetAllBuses()
        {
            List<BusTrip> tempList = GetAllBusTrips().ToList();
            return from BObus in dl.GetAllBuses()
                   select new Bus()
                   {
                       LicenseNumber = BObus.LicenseNumber,
                       Stat = (BusStatus)BObus.Stat,
                       Fuel = BObus.Fuel,
                       LicenesDate = BObus.LicenesDate,
                       LastTreatDate = BObus.LastTreatDate,
                       Kilometraz = BObus.Kilometraz,
                       KmAfterTreat = BObus.KmAfterTreat,
                       BusTrips = tempList.Where(bt => bt.Bus_Id == BObus.LicenseNumber).ToList()
                   };
                   
        }
        public IEnumerable<Bus> GetAllBusesBy(Predicate<Bus> pred)
        {
            return from BObus in dl.GetAllBuses()
                   let bus = (Bus)BObus.CopyPropertiesToNew(typeof(Bus))
                   where pred(bus)
                   select bus;
        }
        public void AddRandomBus()
        {
            AddBus(HelpMethods.RandomBus());
        }
        /// <summary>
        /// reset the buses status(stop from traveling) while closing the propgram
        /// </summary>
        public void ResetBuses()
        {
            garage.ResetBuses();
        }
        #region bus trip
        public void AddBusTrip(BusTrip BusTrip)
        {
            try
            {
                dl.AddBusTrip((DO.BusTrip)BusTrip.CopyPropertiesToNew(typeof(DO.BusTrip)));
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }

        public BusTrip GetBusTrip(int id)
        {
            try
            {
                return (BusTrip)dl.GetBusTrip(id).CopyPropertiesToNew(typeof(BusTrip));
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }

        public void UpdateBusTrip(BusTrip BusTrip)
        {
            try
            {
                dl.UpdateBusTrip((DO.BusTrip)BusTrip.CopyPropertiesToNew(typeof(BO.BusTrip)));
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }

        public void DeleteBusTrip(int id)
        {
            try
            {
                dl.DeleteBusTrip(id);
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }

        public IEnumerable<BusTrip> GetAllBusTrips()
        {
            return from busT in dl.GetAllBusTrips()
                   select (BusTrip)busT.CopyPropertiesToNew(typeof(BusTrip));
        }

        public IEnumerable<BusTrip> GetAllBusTripsBy(Predicate<BusTrip> pred)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region Line
        /// <returns>the Serial number that given to the new line at the data layer</returns>
        public int AddLine(Line line, IEnumerable<Station> stations, List<int?> distances, List<int?> times)
        {
            try
            {
                //add the line:
                DO.Line DoLine = new DO.Line()
                {
                    LineNumber = line.LineNumber,
                    Area = (DO.AreasEnum)Enum.Parse(typeof(DO.AreasEnum), line.Area.ToString()),
                    FirstStation_Id = stations.ElementAt(0).Code,
                    LastStation_Id = stations.Last().Code,
                    IsActive = true
                };
                int lineId = dl.AddLine(DoLine);//add the line and save his id to return it

                //add the addjecent stations:
                List<AdjacentStations> adjStations = Calculate_dist(stations.ToList(), distances, times);//get the list of the AdjacentStations
                foreach (AdjacentStations adjSt in adjStations)//add all the AdjacentStations to dl
                {
                    dl.AddAdjacentStations(new DO.AdjacentStations()
                    {
                        StationCode1 = adjSt.StationCode1,
                        StationCode2 = adjSt.StationCode2,
                        Distance = adjSt.Distance,
                        Time = adjSt.Time,
                        IsActive = true
                    });
                }

                //add tha LineStations:
                DO.LineStation first_station = new DO.LineStation()//first station define sepretly
                {
                    LineId = lineId,
                    StationNumber = stations.ElementAt(0).Code,
                    LineStationIndex = 0,
                    PrevStation = null,
                    Address = stations.ElementAt(0).Address,
                    Name = stations.ElementAt(0).Name,
                    IsActive = true
                };
                DO.LineStation prev_station = first_station;//this will be use to define the filds PrevStation and NextStation in the loop
                stations = stations.Skip(1);//skips the first station in stations (its allready take ceared)
                int index = 1;//this will be use to define the fild LineStationIndex in the loop
                foreach (Station st in stations)//! I think we shuld add in dl function that add a range of LineStation //I don't think so
                {
                    prev_station.NextStation = st.Code;
                    dl.AddLineStation(prev_station);
                    DO.LineStation current = new DO.LineStation()
                    {
                        LineId = lineId,
                        StationNumber = st.Code,
                        LineStationIndex = index++,
                        PrevStation = prev_station.StationNumber,//! I think we shuld add to LineStation id fild
                        Name = st.Name,
                        Address = st.Address,
                        IsActive = true
                    };
                    prev_station = current;
                }
                dl.AddLineStation(prev_station);//add last station

                return lineId;
            }
            catch (Exception msg)
            {

                throw msg;
            }
        }
        public Line GetLine(int id)
        {
            try
            {
                Line line = (Line)dl.GetLine(id).CopyPropertiesToNew(typeof(Line));

                //set the line stations
                List<DO.LineStation> releventLineStations = dl.GetAllLineStationsBy(ls => ls.LineId == id && ls.IsActive).ToList();//set 'releventLineStations' to contin all the line stations of the line
                List <DO.AdjacentStations> releventAdjacentStations =
                    //get all the adjscent stations that required to build this line
                    dl.GetAllAdjacentStationsBy(adjs => releventLineStations.Exists(ls => ls.StationNumber == adjs.StationCode1 && ls.NextStation != null && ls.NextStation == adjs.StationCode2)).ToList();
                line.Stations = LineStations_Do_Bo(releventLineStations, releventAdjacentStations).ToList();

                //set the line tripes
                line.LineTrips = (from DOlineTripe in dl.GetAllLineTripBy(lt => lt.LineId == id)
                                  select new LineTrip()
                                  {
                                      ID = DOlineTripe.ID,
                                      LineId = DOlineTripe.LineId,
                                      StartTime = DOlineTripe.StartTime,
                                      Frequency = DOlineTripe.Frequency,
                                      FinishAt = DOlineTripe.FinishAt
                                  }).ToList();

                return line;
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        /// <summary>
        /// update the stations of the line
        /// </summary>
        public void UpdateLine(int lineId, IEnumerable<Station> stations, List<int?> distances, List<int?> times)
        {
            //get all the line stations of this line from dl
            List<DO.LineStation> oldListOfStations = dl.GetAllLineStationsBy(ls => ls.LineId == lineId && ls.IsActive).ToList();

            //delete all the line stations in this line
            foreach(var st in oldListOfStations)
            {
                dl.DeleteLineStation(lineId, st.StationNumber);
            }

            //add the new list of line stations of the line

            //add the adjacentStations:
            List<AdjacentStations> adjStations = Calculate_dist(stations.ToList(), distances, times);//get the list of the AdjacentStations
            foreach (AdjacentStations adjSt in adjStations)//add all the AdjacentStations to dl
            {
                dl.AddAdjacentStations(new DO.AdjacentStations()
                {
                    StationCode1 = adjSt.StationCode1,
                    StationCode2 = adjSt.StationCode2,
                    Distance = adjSt.Distance,
                    Time = adjSt.Time,
                    IsActive = true
                });
            }

            //add tha LineStations:
            DO.LineStation first_station = new DO.LineStation()//first station define sepretly
            {
                LineId = lineId,
                StationNumber = stations.ElementAt(0).Code,
                LineStationIndex = 0,
                PrevStation = null,
                IsActive = true
            };
            DO.LineStation prev_station = first_station;//this will be use to define the filds PrevStation and NextStation in the loop
            stations = stations.Skip(1);//remove the first station from stations (its allready take ceared)
            int index = 1;//this will be use to define the fild LineStationIndex in the loop
            foreach (Station st in stations)//! I think we shuld add in dl function that add a range of LineStation
            {
                prev_station.NextStation = st.Code;
                dl.AddLineStation(prev_station);
                DO.LineStation current = new DO.LineStation()
                {
                    LineId = lineId,
                    StationNumber = st.Code,
                    LineStationIndex = index++,
                    PrevStation = prev_station.StationNumber,//! I think we shuld add to LineStation id fild
                    IsActive = true
                };
                prev_station = current;
            }
            dl.AddLineStation(prev_station);//add last station
        }
        public void DeleteLine(int id)
        {
            Line line = GetLine(id);
            try
            {
                delete_lineStations(id, line.Stations.Select(ls => ls.StationNumber).ToList());//delete all the lineStations of the line
                delete_lineTrips(line.LineTrips.Select(lt => lt.ID).ToList());//delete all the lineTrips of the line
                dl.DeleteLine(id);
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        public IEnumerable<Line> GetAllLines()
        {
            try
            {
                var LineStationsGroups = dl.GetAllLineStationsBy(lst => lst.IsActive).GroupBy(lst => lst.LineId).ToArray();//get all line stations and goup them by they Line id
                List<DO.AdjacentStations> allAdjacentStations = dl.GetAllAdjacentStationsBy(adjSt=>true).ToList();
                List<DO.LineTrip> AllLineTrips = dl.GetAllLineTripBy(lt => lt.IsActive).ToList();
                IEnumerable<Line> result = from doLine in dl.GetAllLinesBy(l => l.IsActive)
                                              //prepare the LineStations
                                              let doStations = LineStationsGroups.Where(gr => gr.Key == doLine.ID).FirstOrDefault().ToList()//get the list of the LineStation of the current line
                                              let boStations = LineStations_Do_Bo(doStations, allAdjacentStations)//convert the LineStations to BO's LineStations 
                                              //Prepare the line trips
                                              let doLineTrips = AllLineTrips.Where(lt => lt.LineId == doLine.ID)//extract from allLinetrips the lineTrips of ths line
                                              let boLineTrips = doLineTrips.Select(doLt => new LineTrip()//convert to BO.Linetrip
                                                                                                          {
                                                                                                            ID = doLt.ID,
                                                                                                            LineId = doLt.LineId,
                                                                                                            StartTime = doLt.StartTime,
                                                                                                            Frequency = doLt.Frequency,
                                                                                                            FinishAt = doLt.FinishAt
                                                                                                          })
                                              select new Line()
                                              {
                                                  ID = doLine.ID,
                                                  LineNumber = doLine.LineNumber,
                                                  Area = (AreasEnum)Enum.Parse(typeof(AreasEnum), doLine.Area.ToString()),
                                                  Stations = boStations,
                                                  LineTrips = boLineTrips.ToList()
                                              };
                return result;
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        public IEnumerable<Line> GetAllLinesBy(Predicate<Line> pred)
        {
            try
            {
                return from lineDO in dl.GetAllLines()
                       let lineBO = GetLine(lineDO.ID)
                       where pred(lineBO)
                       select lineBO;
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        #endregion

        #region LineStation
        public void AddLineStation(int lineId, int stationNumber, int index)
        {
            Line line = GetLine(lineId);
            if (index < 0 || index > line.Stations.Count)
            {
                throw new IndexOutOfRangeException("the index is out of range");
            }
            DO.LineStation lineStationDO = new DO.LineStation()
            {
                LineId = lineId,
                StationNumber = stationNumber,
                LineStationIndex = index,
                PrevStation = index == 0 ? null : (int?)line.Stations[index - 1].StationNumber,
                NextStation = index == line.Stations.Count ? null : (int?)line.Stations[index].StationNumber,
                IsActive = true
            };

            //generating the BO line station
            LineStation lineStation = (LineStation)lineStationDO.CopyPropertiesToNew(typeof(LineStation));//copy from DO line station          
            lineStation = (LineStation)GetStation(stationNumber).CopyPropertiesToNew(typeof(LineStation));//copy from BO station          

            if (HelpMethods.AddAdjacentStations(lineStationDO.PrevStation, lineStationDO.NextStation))//if success to add "Adjacent Stations" (it's mean the line station is not the first station)
                lineStation.PrevToCurrent = HelpMethods.GetAdjacentStations(lineStationDO.PrevStation, lineStationDO.NextStation);

            if (HelpMethods.AddAdjacentStations(lineStationDO.NextStation, lineStationDO.PrevStation))//if success to add "Adjacent Stations" (it's mean the line station is not the last station)
                lineStation.CurrentToNext = HelpMethods.GetAdjacentStations(lineStationDO.NextStation, lineStationDO.PrevStation);

            //updating the stations's index
            for (int i = index; i < line.Stations.Count; i++)//updates the indexes of the stations
            {
                line.Stations[i].LineStationIndex++;
            }
            line.Stations.Insert(index, lineStation);

            if (lineStation.PrevToCurrent != null)//the added station is not in the first location
            {
                HelpMethods.DeleteAdjacentStations(line.Stations[index - 1].CurrentToNext);
                line.Stations[index - 1].CurrentToNext = line.Stations[index].PrevToCurrent;
            }

            if (lineStation.CurrentToNext != null)//the added station is not in the last location
            {
                HelpMethods.DeleteAdjacentStations(line.Stations[index + 1].PrevToCurrent);
                line.Stations[index + 1].PrevToCurrent = line.Stations[index].CurrentToNext;
            }

            dl.AddLineStation(lineStationDO);
        }
        public void UpdateLineStation(int lineNumber, int StationNumber)
        {
            throw new NotImplementedException();
        }
        public void DeleteLineStation(int lineNumber, int StationNumber)
        {
            Line line = GetLine(lineNumber);

            line.Stations.OrderBy(ls => ls.LineStationIndex);//Make sure the line stations in 'line' are ordered by the fild 'LineStationIndex'

            LineStation lineStation = line.Stations.FirstOrDefault(ls => ls.StationNumber == StationNumber);

            int indexInLine = lineStation.LineStationIndex;

            //the deleted station is the first stop of the line
            if (indexInLine == 0)
            {
                //update the second line station of the line (now is the first line station)
                LineStation temp = line.Stations[1];//for convenience save a reference to the second line station in the line(now the first line station)
                dl.UpdateLineStation(ls => ls.PrevStation = null, line.ID, temp.StationNumber);//set the fild 'PrevStation' of the second line station in line to be null because now its the first line station in line

                //update the line (the FirstStation_Id fild)
                dl.UpdateLine(new DO.Line()
                {
                    ID = line.ID,
                    LineNumber = line.LineNumber,
                    Area = (DO.AreasEnum)Enum.Parse(typeof(DO.AreasEnum), line.Area.ToString()),//Area = line.Area
                    LastStation_Id = line.LastStation.StationNumber,
                    FirstStation_Id = line.Stations[1].StationNumber,//the second station in the line now is the first station
                    IsActive = true
                });

            }
            //if the station is the last stop of the line
            else if (indexInLine == line.Stations.Count() - 1)
            {
                LineStation temp = line.Stations[indexInLine - 1];//for convenience save a reference to second from last line station in the line(now the last line station)
                dl.UpdateLineStation(ls => ls.NextStation = null, line.ID, temp.StationNumber);//set the fild 'NextStation' of the second from end line station to be null because now its the last line station in line

                //update the line (the LastStation_Id fild)
                dl.UpdateLine(new DO.Line()
                {
                    ID = line.ID,
                    LineNumber = line.LineNumber,
                    Area = (DO.AreasEnum)Enum.Parse(typeof(DO.AreasEnum), line.Area.ToString()),//Area = line.Area
                    LastStation_Id = line.Stations[lineStation.LineStationIndex - 1].StationNumber,//now the last station is the station before the delets line station
                    FirstStation_Id = line.FirstStation.StationNumber,
                    IsActive = true
                });
            }
            //if the line station to delete is not the first and not the last in the line
            else
            {
                //save arefernce to the preius station and to the next for convenience
                LineStation prevInLine = line.Stations[indexInLine - 1];
                LineStation nextInLine = line.Stations[indexInLine + 1];

                //update the privius and next stations of the line:
                //privius station:
                dl.UpdateLineStation(ls => ls.NextStation = nextInLine.StationNumber, line.ID, prevInLine.StationNumber);
                //next station:
                dl.UpdateLineStation(ls => ls.PrevStation = prevInLine.StationNumber, line.ID, nextInLine.StationNumber);


                //add new adjecent station for (prevInLine, nextInLine)
                dl.AddAdjacentStations(new DO.AdjacentStations()
                {
                    StationCode1 = prevInLine.StationNumber,
                    StationCode2 = nextInLine.StationNumber,
                    Distance = lineStation.PrevToCurrent.Distance + lineStation.CurrentToNext.Distance,//rughly assuming that the distance between the 'prevInLine' and the 'nextInLine' is the sum of the distances from them to the deletes line station
                    Time = lineStation.PrevToCurrent.Time + lineStation.CurrentToNext.Time,//same as above^^^^
                    IsActive = true
                });
            }

            //update the index of all the line stations after the deletes line station to be less in 1(LineStationIndex--)
            for (int i = indexInLine; i < line.Stations.Count(); i++)
            {
                dl.UpdateLineStation(ls => ls.LineStationIndex--, line.ID, line.Stations[i].StationNumber);
            }

            dl.DeleteLineStation(line.ID, lineStation.StationNumber);//Finally delete the line station 
        }
        void delete_lineStations(int lineId, List<int> lineStationsNumbers)
        {
            foreach (var lsNumber in lineStationsNumbers)
            {
                try
                {
                    dl.DeleteLineStation(lineId, lsNumber);
                }
                catch (DO.NotExistExeption)
                {
                    continue;
                    throw;
                }
            }
        }
        #endregion

        #region Line trip
        public void AddLineTrip(LineTrip lineTrip)
        {
            try
            {
                dl.AddLineTrip((DO.LineTrip)lineTrip.CopyPropertiesToNew(typeof(DO.LineTrip)));
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        public LineTrip GetLineTrip(int id)
        {
            try
            {
                return (LineTrip)dl.GetLineTrip(id).CopyPropertiesToNew(typeof(LineTrip));
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        public void UpdateLineTrip(LineTrip lineTrip)
        {
            try
            {
                dl.UpdateLineTrip((DO.LineTrip)lineTrip.CopyPropertiesToNew(typeof(DO.LineTrip)));
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        public void DeleteLineTrip(LineTrip lineTrip)
        {
            try
            {
                dl.DeleteLineTrip(lineTrip.ID);
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        void delete_lineTrips(List<int> lineTripsIds)
        {
            foreach (var ltId in lineTripsIds)
            {
                dl.DeleteLineTrip(ltId);
            }
        }
        public IEnumerable<LineTrip> GetAllLineTrips()
        {
            return from LTripDO in dl.GetAllLineTrips()
                   select (LineTrip)LTripDO.CopyPropertiesToNew(typeof(LineTrip));
        }
        public IEnumerable<LineTrip> GetAllLineTripBy(Predicate<LineTrip> predicate)
        {
            return from LTripDO in dl.GetAllLineTrips()
                   let LTripBO = (LineTrip)LTripDO.CopyPropertiesToNew(typeof(LineTrip))
                   where predicate(LTripBO)
                   select LTripBO;
        }
        public List<Ride> GetRides(LineTrip lineTrip)
        {
            List<Ride> rides = new List<Ride>();
            for (TimeSpan rideTime = lineTrip.StartTime; rideTime <= lineTrip.FinishAt; rideTime += lineTrip.Frequency)//this is all the rides of this line trip in one day
            {
               rides.Add(new Ride()
                {
                    LineId = lineTrip.LineId,
                    LineTripId = lineTrip.ID,
                    StartTime = rideTime,
                });
            }
            return rides;
        }

        #endregion

        #region Passenger
        public Passenger GetPassenger(string name, string password)
        {
            throw new NotImplementedException();
        }
        public void AddPassenger(string name, string password)
        {
            throw new NotImplementedException();
        }
        public void UpdatePassenger(string name, string password, string newName, string newPassword)
        {
            throw new NotImplementedException();
        }
        public void DeletePassenger(string name, string password)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Station

        /// <summary>
        /// adds new station to the data base
        /// </summary>
        /// <param name="station">the station to add</param>
        /// <exception cref="LocationOutOfRange">if the location of the station is not in the allowable range</exception>
        public void AddStation(Station station)
        {
            const double max_latitude = 33.3;
            const double min_latitude = 31;
            const double max_longitude = 35.5;
            const double min_longitude = 34.3;


            //check if the location is valid
            if (station.Longitude > max_longitude || station.Longitude < min_longitude ||
               station.Latitude > max_latitude || station.Latitude < min_latitude)
            {
                throw new LocationOutOfRange($"unvalid location, location range is: longitude: [{min_longitude} - {max_longitude}] ,latitude: [{min_latitude} - {max_latitude}]");
            }

            //creates a DO.Station to add to dl
            DO.Station DOstation = (DO.Station)station.CopyPropertiesToNew(typeof(DO.Station));

            try
            {
                dl.AddStation(DOstation);
            }
            catch (DO.DuplicateExeption)//if station with identical code allready exist
            {
                throw new DuplicateExeption("station with identical code allready exist");
            }
        }
        public Station GetStation(int code)
        {
            try
            {
                Station station = (Station)dl.GetStation(code).CopyPropertiesToNew(typeof(Station));
                station.LinesNums = (from lineNum in dl.GetAllLineStationsBy(s => s.LineId == code)
                                     orderby lineNum.LineId
                                     select lineNum.LineId).ToList();
                return station;
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        public void UpdateStation(Station station)
        {
            dl.UpdateStation(new DO.Station()
            {
                Code = station.Code,
                Name = station.Name,
                Longitude = station.Longitude,
                Latitude = station.Latitude,
                Address = station.Address,
            });
        }
        public void DeleteStation(int code)
        {
            try
            {
                foreach (DO.LineStation lineS in dl.GetAllLineStationsBy(s => s.StationNumber == code))//delete all the line stations of the deleted station from all the lines
                {
                    DeleteLineStation(lineS.LineId, lineS.StationNumber);
                }
                foreach (DO.AdjacentStations AjaS in dl.GetAllAdjacentStationsBy(a => a.StationCode1 == code || a.StationCode2 == code))//delete all the adjecentStations that involve the deleted station
                {
                    dl.DeleteAdjacentStations(AjaS);
                }
                dl.DeleteStation(code);//delete the station
            }
            catch (Exception msg)
            {
                throw msg;
            }
        }
        public IEnumerable<Station> GetAllStations()
        {
            List<DO.LineStation> lineStations = dl.GetAllLineStations().ToList();//to proserv time it's loading all the line stations ahed
            return from DOst in dl.GetAllStations()
                   select new Station()
                   {
                       Code = DOst.Code,
                       Name = DOst.Name,
                       Location = new GeoCoordinate(DOst.Latitude,
                       DOst.Longitude),
                       Address = DOst.Address,
                       LinesNums = lineStations.Where(lst => lst.StationNumber == DOst.Code).Select(lst => lst.LineId).ToList()//find all the lineStations that conected to this station and get they lineId
                   };
        }
        public IEnumerable<Station> GetAllStationsBy(Predicate<Station> pred)
        {
            return from stationDO in dl.GetAllStations()
                   let stationBO = GetStation(stationDO.Code)
                   where pred(stationBO)
                   select stationBO;
        }
        #endregion

        #region User Trip
        public void AddUserTrip(UserTrip userTrip)
        {
            throw new NotImplementedException();
        }

        public UserTrip GetUserTrip(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserTrip(UserTrip userTrip)
        {
            throw new NotImplementedException();
        }

        public void DeleteUserTrip(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserTrip> GetAllUserTrips(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserTrip> GetAllUserTripsBy(string name, Predicate<UserTrip> pred)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region simulator
        SimulationClock clock = SimulationClock.Instance;
        TravelsExecuter travelsExecuter = TravelsExecuter.Instance;
        /// <summary>
        /// start the simulatorClock and the travel executer
        /// </summary>
        /// <param name="startTime">the time wich the simolator clock will start from</param>
        /// <param name="Rate">the rate of the simulator clock relative to real time</param>
        /// <param name="updateTime">will executet when the simulator time changes</param>
        public void StartSimulator(TimeSpan startTime, int rate, Action<TimeSpan> updateTime, Action<LineTiming> updateBus, Action<BusProgress> busObserver, Action<Exception> ExptionsObserver)
        {
            Garage.Instance.Observer = busObserver;
            clock.StartClock(startTime, rate, updateTime);
            travelsExecuter.StartExecute(updateBus, busObserver, ExptionsObserver);
        }

        /// <summary>
        /// stops the simulator clock and the travels executer and all the travels that in progres
        /// </summary>
        public void StopSimulator()
        {
            clock.StopClock();//this stops the travels executer too
        }

        /// <summary>
        /// adds the station to the list of the stations that under truck
        /// </summary>
        public void Add_stationPanel(int stationCode)
        {
            travelsExecuter.Add_station_to_truck(stationCode);
        }

        /// <summary>
        /// removes the station from the list of the stations that under truck
        /// </summary>
        public void Remove_stationPanel(int stationCode)
        {
            travelsExecuter.Add_station_to_truck(stationCode);
        }

        public void Change_SimulatorRate(int change)
        {
            clock.Change_Rate(change);
        }

        public void Refuel(Bus bus)
        {
            try
            {
                garage.Refule(bus);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public void Treatment(Bus bus)
        {
            try
            {
                garage.Treatment(bus);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        #endregion

        #region private methods

        #region calculation methods

        /// <returns>colection of AdjacentStations accurding to this structer: { (st[1],st[2]), (st[2], st[3]),...,(st[n-1], st[n]) } where st stend for stations</returns>
        List<AdjacentStations> Calculate_dist(List<Station> stations, List<int?> distances, List<int?> times)
        {
            List<AdjacentStations> result = new List<AdjacentStations>();
            Station pre_station = stations[0];
            stations.RemoveAt(0);//remove the first station fro stations
            int i = 0;//this will be use to add the correct distance and time to the adjasentStations acording to 'distances' and 'times'
            foreach (Station current in stations)//starting from the second station in the resepted list, the first is in prev_station
            {
                result.Add(new AdjacentStations()
                {
                    StationCode1 = pre_station.Code,
                    StationCode2 = current.Code,
                    Distance = distances[i] != null? (int)distances[i] :GetDist(pre_station, current),//if the user inserted distance manualy then take the inserted distanse, else calculate defult distance
                    Time = times[i] != null? new TimeSpan(0,(int)times[i],0) : GetTime(pre_station, current, GetDist(pre_station, current))//if the user inserted time manualy then take the inserted time, else calculate defult time
                });
                pre_station = current;
                i++;
            }
            return result;
        }
        static Random r = new Random(DateTime.Now.Millisecond);
        private TimeSpan GetTime(Station station1, Station station2, double dist)
        {
            double rand = r.NextDouble() * 1.8 + 1.2;//random number in the range 1.2 - 3
            return TimeSpan.FromMinutes(rand * dist);//calculating the time by distance driving around 20 - 50 kmh
        }

        private double GetDist(Station station1, Station station2)
        {
            return station1.Location.GetDistanceTo(station2.Location)/1000;
        }
        #endregion

        #region clone methods
        /// <summary>
        /// take a list of DO's LineStation and convert them to BO's LineStation,  
        /// assuming all the line stations is from the same line!!!
        /// </summary>
        List<LineStation> LineStations_Do_Bo(List<DO.LineStation> lineStations, List<DO.AdjacentStations> adjacentStations)//^^^^^^^^^^^^continue from heare^^^^^^^^^^^^
        {
            //check that all the stationLines from the same line
            if(lineStations.Exists(lst => lst.LineId != lineStations[0].LineId))
            {
                throw new invalidUseOfFunc("all the LineStations need to be from the same line");
            }

            List<LineStation> result = new List<LineStation>();
            lineStations.OrderBy(lst => lst.LineStationIndex);//order by the index of the lineStation in the line
            double distance_from_start = 0;
            TimeSpan time_from_start = new TimeSpan(0);
            LineStation first_station = new LineStation()
            {
                LineId = lineStations[0].LineId,
                StationNumber = lineStations[0].StationNumber,
                LineStationIndex = lineStations[0].LineStationIndex,
                PrevToCurrent = null,//ther is no previus station so it's null
                Address = lineStations[0].Address,
                Name = lineStations[0].Name,
                Distance_from_start = distance_from_start,// = 0// because it's the first station
                Time_from_start = time_from_start// = 0:0:0// because it's the first station
                //the fild CurrentToNext will be set inside the loop
            };
            lineStations.RemoveAt(0);//remove the first line station for its been Taken care allready
            LineStation prev_lineStation = first_station;//this will be use to set the fild CurrentToNext in the loop
            foreach (DO.LineStation lst in lineStations)
            {
                var temp = adjacentStations.FirstOrDefault(adjs => adjs.StationCode1 == prev_lineStation.StationNumber && adjs.StationCode2 == lst.StationNumber);
                if(temp == null)
                {
                    throw new missAdjacentStations($"miss adjacentStations ({prev_lineStation.StationNumber}, {lst.StationNumber}");
                }
                AdjacentStations prev_to_current = (AdjacentStations)temp.CopyPropertiesToNew(typeof(AdjacentStations));//I used temp to shorts the row

                distance_from_start += prev_to_current.Distance;//add the distance from the previus station to distanse from start
                time_from_start += prev_to_current.Time;//        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

                prev_lineStation.CurrentToNext = prev_to_current;//the fild PrevToCurrent of current is CurrentToNext of prev_lineStation
                result.Add(prev_lineStation);
                prev_lineStation = new LineStation()
                {
                    LineId = lst.LineId,
                    StationNumber = lst.StationNumber,
                    LineStationIndex = lst.LineStationIndex,
                    Address = lst.Address,
                    Name = lst.Name,
                    PrevToCurrent = prev_to_current,
                    Distance_from_start = distance_from_start,
                    Time_from_start = time_from_start
                };
                
            }
            result.Add(prev_lineStation);
            return result;
        }

        public IEnumerable<LineStation> GetAllLineStations()
        {
            throw new NotImplementedException();
        }

        public List<TimeTrip> CalculateTimeTrip(List<LineStation> lineStation)
        {
            throw new NotImplementedException();
        }

        public List<TimeTrip> CalculateTimeTrip(LineStation lineStation, int lineNum)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
