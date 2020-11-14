﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNet5781_02_1038_0685
{
    public struct Point
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        static public bool operator ==(Point a, Point b)
        {
            return ((a.Latitude == b.Latitude) && (a.Longitude == b.Longitude));
        }
        static public bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }
    }
    public class Station
    {
        #region static
        #region fildes
        private static List<Station> usedcodes = new List<Station>();
        #endregion

        #region fanctions
        public static Station get_station(int code)
        {
            Station temp = usedcodes.Find((Station st) => st.StationCode == code);
            if (temp == null)
            {
                throw new ArgumentException("this station do not exist");
            }
            return temp;
        }  
        #endregion

        #endregion

        #region CONSTANTS
        private const int SIXDIGITS = 1000000;
        private const int MIN_LAT = -90;
        private const int MAX_LAT = 90;
        private const int MIN_LON = -180;
        private const int MAX_LON = 180;
        #endregion

        #region private fields
        private int stationCode;
        private List<BusLine> pass_here;
        #endregion

        #region propertys
        public int StationCode
        {
            get => this.stationCode;
            protected set
            {
                if (usedcodes.Exists((Station st) => st.StationCode == value))
                {
                    throw new ArgumentException("this code allready whas taken");
                }
                if (value < 0 || value > SIXDIGITS)
                {
                    throw new ArgumentException("unvalid id");
                }
                else
                {
                    this.stationCode = value;
                    usedcodes.Add(this);
                }
            }
        }

        private List<BusLine> Pass_here
        {
            get => this.pass_here;
            set => pass_here = value;
        }


        private Point loc;
        protected Point Loc
        {
            get => loc;
            set
            {
                if (value.Latitude < -90 || value.Longitude < -180 || value.Latitude > 90 || value.Longitude > 180)
                {
                    throw new ArgumentException(string.Format("latitude must be between {0} and {1}, longitude must be between {2} and {3}", MIN_LAT, MAX_LAT, MIN_LON, MAX_LON));
                }
                else
                {
                    loc = value;
                }
            }
        }

        protected string Address { get; set; }
        #endregion

        
        public Station(int code, double latitude, double longitude, string address = "")
        {
            StationCode = code;
            Loc = new Point { Latitude = latitude, Longitude = longitude };//*Point is astruct
            Address = address;
            usedcodes.Add(this);
        }

        public void Add_line(BusLine bl)
        {
            pass_here.Add(bl);
        }

        public override string ToString()
        {
            return string.Format("Station code: {0}, {1}°N {2}°E", stationCode, loc.Latitude, loc.Longitude);
        }

    }
}
