﻿using System;
using System.Collections.Generic;

using DO;

namespace DLApi
{
    public interface IDL
    {
        #region Bus
        /// <summary>
        /// Add DO bus
        /// </summary>
        /// <exception cref="DuplicateExeption">
        /// </exception>
        void AddBus(Bus bus);
        /// <summary>
        /// Get DO bus
        /// </summary>
        /// <exception cref="NotExistExeption">
        /// </exception>
        Bus GetBus(string licenseNum);
        /// <summary>
        /// Update DO bus
        /// </summary>
        /// <exception cref="NotExistExeption">
        /// </exception>
        void UpdateBus(Bus bus);
        /// <summary>
        /// Delete DO bus
        /// </summary>
        /// <exception cref="NotExistExeption">
        /// </exception>
        void DeleteBus(string licenseNum);
        IEnumerable<Bus> GetAllBuses();
        IEnumerable<Bus> GetAllBusesBy(Predicate<Bus> predicate);
        #endregion

        #region Line
        /// <summary>
        /// add DO line
        /// </summary>
        int AddLine(Line line);

        /// <summary>
        /// Ged DO Line with Id = 'id'
        /// </summary>
        Line GetLine(int id);

        /// <summary>
        /// update DO Line
        /// </summary>
        void UpdateLine(Line Line);

        /// <summary>
        /// delete DO line
        /// </summary>
        /// <param name="id"></param>
        void DeleteLine(int id);

        /// <summary>
        /// Get All DO lines
        /// </summary>
        /// <returns></returns>
        IEnumerable<Line> GetAllLines();
        /// <summary>
        /// Get All the lines that predicate returns true for them
        /// </summary>
        IEnumerable<Line> GetAllLinesBy(Predicate<Line> predicate);
        /// <summary>
        /// return line number according the line id
        /// </summary>
        int GetLineNum(int id);

        #endregion

        #region BusTrip
        /// <summary>
        /// add DO BusTrip
        /// </summary>
        void AddBusTrip(BusTrip busTrip);

        /// <summary>
        /// get DO BusTrip
        /// </summary>
        BusTrip GetBusTrip(int id);

        /// <summary>
        /// Update DO BusTrip
        /// </summary>
        void UpdateBusTrip(BusTrip busTrip);

        /// <summary>
        /// delete busTrip
        /// </summary>
        void DeleteBusTrip(int id);

        /// <summary>
        /// returns all LineTrips in the data base
        /// </summary>
        IEnumerable<BusTrip> GetAllBusTrips();

        /// <summary>
        /// returns all the busTrips that predicate returns true for them
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<BusTrip> GetAllBusTripsBy(Predicate<BusTrip> predicate);
        #endregion

        #region Station
        /// <summary>
        /// add station to the data source</br>
        /// if ther is station with same code DuplicateExeption will be throw
        /// </summary>
        /// <param name="busStation">station to add</param>
        void AddStation(Station busStation);
        /// <summary>
        /// return a clone of the station with Code = 'code' in the data source</br>
        /// if ther is no such station NotExistExeption will be throw
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Station GetStation(int code);
        /// <summary>
        /// override the station with the same code in the data sorce</br>
        /// if the old station dont exist NotExistExeption will be throw
        /// </summary>
        void UpdateStation(Station busStation);
        /// <summary>
        /// unactivate the station with Code = 'code' from the data source</br>
        /// if the station dont exist or allready unactive NotExistExeption will be throw
        /// </summary>
        /// <param name="code"></param>
        void DeleteStation(int code);
        /// <summary>
        /// returns a clones of all the active stations
        /// </summary>
        IEnumerable<Station> GetAllStations();
        /// <summary>
        /// return aclone of all the stations that predicate returns true for them
        /// </summary>
        IEnumerable<Station> GetAllStationBy(Predicate<Station> predicate);
        #endregion

        #region LineStation
        /// <summary>
        /// add new line station to the data source.</br>
        /// if ther is allready line station with identical line id and station number:</br>
        /// if it active DuplicateExeption will be throw.</br>
        /// else, overrides the unactive line station with the new one
        /// </summary>
        /// <param name="lineStation">line station to add</param>
        void AddLineStation(LineStation lineStation);
        /// <summary>
        /// returns a clone of the line station in the data source with LineId = 'lineId' and StationNumber = 'stationNum'</br>
        /// if the line station dont exist NotExistExeption will be throw
        /// </summary>
        LineStation GetLineStation(int lineId, int stationNum);
        /// <summary>
        /// returns a clone of line station with LineId == 'lineId' and LineStationIndex == 'index'
        /// if the line station dont exist NotExistExeption will be throw
        /// </summary>
        LineStation GetLineStationByIndex(int lineId, int index);
        /// <summary>
        /// overrides the old line station with LineId = 'lineId' and StationNumber = 'stationNum' with the new line station</br>
        /// </summary>
        /// <param name="newLineStation">up to date line station</param>
        void UpdateLineStation(LineStation lineStation);
        /// <summary>
        /// <br>preform 'action' on the line station with 'LineId' = 'lineId' and 'StationNumber' = stationNumber</br>
        /// and override the line station with the result of the action
        /// </summary>
        void UpdateLineStation(Action<LineStation> action, int lineId, int stationNumber);
        /// <summary>
        /// <br>delete the line station with LineId = 'lineId' and StationNumber = 'stationNum'</br>
        /// </summary>
        /// <param name="lineId"></param>
        /// <param name="stationNum"></param>
        void DeleteLineStation(int lineId, int stationNum);
        /// <summary>
        /// returns a clone of all the line stations
        /// </summary>
        /// <returns></returns>
        IEnumerable<LineStation> GetAllLineStations();
        /// <summary>
        /// return a clone of all the line station that predicate returns tru for them
        /// </summary>
        IEnumerable<LineStation> GetAllLineStationsBy(Predicate<LineStation> predicate);
        #endregion

        #region AdjacentStations
        /// <summary>
        /// <br>add a new adjacent stations to the data source</br>
        /// </summary>
        /// <param name="adjacentStations">new adjacent stations to add</param>
        void AddAdjacentStations(AdjacentStations adjacentStations);
        /// <summary>
        /// returns a clone of the adjacent stations with StationCode1 = 'stationCode1' and StationCode2 = 'stationCode2'
        /// </summary>
        AdjacentStations GetAdjacentStation(int? stationCode1, int? stationCode2);
        /// <summary>
        /// overrids old adjacent stations with same stationCode1 and stationCode2
        /// </summary>
        /// <param name="newAdjacentStations">up to date station</param>
        void UpdateAdjacentStations(AdjacentStations newAdjacentStations);
        /// <summary>
        /// try to delete the Adjacent Stations
        /// </summary>
        /// <returns>
        /// <br>true: if success.</br>  <br>false: if any of the given parameters is null</br>
        /// </returns>
        bool DeleteAdjacentStations(int? stationCode1, int? stationCode2);
        /// <summary>
        /// try to delete the Adjacent Stations
        /// </summary>
        /// <returns>
        /// <br>true: if success.</br>  <br>false: if any of the adjacentStations doesn't exist</br>
        /// </returns>
        bool DeleteAdjacentStations(AdjacentStations adjacentStations);
        ///<returns>a clones of all the objects where predicate returns true for them(active and not active)</returns>
        IEnumerable<AdjacentStations> GetAllAdjacentStationsBy(Predicate<AdjacentStations> predicate);

        #endregion

        #region LineTrip
        /// <summary>
        /// add new line trip to the data source
        /// </summary>
        /// <param name="lineTrip">new line trip to add</param>
        /// <returns>the serial number of this line trip</returns>
        int AddLineTrip(LineTrip lineTrip);
        /// <summary>
        /// <br>returns a clone of a line trip withe ID = 'id' from the data source</br>
        /// <br>if such line trip dont exist NotExistExeption will be throw</br>
        /// </summary>
        LineTrip GetLineTrip(int id);
        /// <summary>
        /// <br></br>overrides the old line trip with same id</br>
        /// <br>if such line trip dont exist NotExistExeption will be throw</br>
        /// </summary>
        /// <param name="newLineTrip"></param>
        void UpdateLineTrip(LineTrip lineTrip);
        void DeleteLineTrip(int lineTrip);
        /// <summary>
        /// returns a clones of all the active line trips in the data source
        /// </summary>
        IEnumerable<LineTrip> GetAllLineTrips();
        /// <summary>
        /// returns a clones of all the line trips that predicate return true for them
        /// </summary>
        IEnumerable<LineTrip> GetAllLineTripBy(Predicate<LineTrip> predicate);
        #endregion

        #region User
        /// <summary>
        /// <br>adds new user to data source</br>
        /// <br>if ther is allready unactive user with the same name then the new user will override th unactive one</br>
        /// </summary>
        /// <param name="user">new user to add</param>
        /// <exception cref="DuplicateExeption">if ther is an active user with the same Name allready in the data source</exception>
        void AddUser(User user);
        /// <summary>
        /// returns a clone of the user with Name = 'name'
        /// </summary>
        /// <exception cref="NotExistExeption">if the user dont exist or un active</exception>
        User GetUser(string name);
        /// <summary>
        /// overrides the user with the same name in the data source with the up to date user
        /// </summary>
        /// <exception cref="NotExistExeption">if ther is no such user in the data source</exception>
        void UpdateUser(User user);
        /// <summary>
        /// unactivate the user in the data source with Name = 'name'
        /// </summary>
        /// <exception cref="NotExistExeption">if ther is no such user in the data source or the user is all ready unactive</exception>
        void DeleteUser(string name);
        #endregion

        #region UserTrip
        /// <summary>
        /// add new DO UserTrip
        /// </summary>
        int AddUserTrip(UserTrip userTrip);

        /// <summary>
        /// Get Do UserTrip
        /// </summary>
        UserTrip GetUserTrip(int id);

        /// <summary>
        /// Update UserTrip
        /// </summary>
        void UpdateUserTrip(UserTrip userTrip);

        /// <summary>
        /// Get all the user trips in the data base
        /// </summary>
        IEnumerable<UserTrip> GetAllUserTrips();

        /// <summary>
        /// get all the userTrips that predicate returns true for them
        /// </summary>
        IEnumerable<UserTrip> GetAllUserTripsBy(Predicate<UserTrip> predicate);
        #endregion
    }
}
