﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace PLGui.Models.PO
{
    class LineStation : ObservableObject
    {
        private Station station;

        public Station Station
        {
            get => station;
            set
            {
                SetProperty(ref station, value);
            }
        }

        private int distance;

        public int Distance
        {
            get => distance;
            set
            {
                SetProperty(ref distance, value);
            }
        }

        private int time;

        public int Time
        {
            get => time;
            set
            {
                SetProperty(ref time, value);
            }
        }

    }
}