﻿namespace BO
{
    public class LineStation
    {
        public int LineId { get; set; }
        public int StationNumber { get; set; }
        public int LineStationIndex { get; set; }
        public int Location { get; set; }
        public string Address { get; set; }

        public AdjacentStations PrevToCurrent { get; set; }
        public AdjacentStations CurrentToNext { get; set; }
        
        public double DistanceBack { get => PrevToCurrent.Distance; }
        public double DistanceNext { get => CurrentToNext.Distance; }



    }
}