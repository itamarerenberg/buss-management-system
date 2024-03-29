﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace PLGui.Models.PO 
{
    public class Line : ObservableValidator
    {

        public Line()
        {
            this.ErrorsChanged += Line_ErrorsChanged;
            this.PropertyChanged += Line_PropertyChanged;
        }

        private int id;
        public int ID 
        {
            get => id;
            set => SetProperty(ref id, value, true);
        }

        private int? lineNumber;
        [Required(ErrorMessage = "line number cannot be empty")]
        [Range(1, 999, ErrorMessage = "line number should be between 1 - 999")]
        public int? LineNumber 
        {
            get => lineNumber;
            set =>SetProperty(ref lineNumber, value, true);
        }

        private AreasEnum area;
        public AreasEnum Area 
        {
            get => area;
            set => SetProperty(ref area, value, true);
        }

        private ObservableCollection<BO.LineStation> stations;
        [MinLength(2, ErrorMessage = "the line has to contains at least 2 stations")]
        public ObservableCollection<BO.LineStation> Stations
        {
            get
            {
                stations.OrderBy(st=>st.LineStationIndex);
                return stations;
            }
            set
            {
                stations = value;
                OnPropertyChanged(nameof(stations));
                OnPropertyChanged(nameof(LastStation));
                OnPropertyChanged(nameof(FirstStation));
            }
        }

        public BO.LineStation FirstStation { get => Stations.First(); }
        public BO.LineStation LastStation { get => Stations.Last(); }
        public ObservableCollection<int> StationsNums { get => new ObservableCollection<int>(Stations.Select(c => c.StationNumber)) { }; }

        private ObservableCollection<LineTrip> lineTrips;

        public ObservableCollection<LineTrip> LineTrips
        {
            get => lineTrips;
            set => SetProperty(ref lineTrips, value);
        }

        private void Line_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(HasErrors))
            {
                OnPropertyChanged(nameof(HasErrors)); // Update HasErrors on every change
            }
        }

        private void Line_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            //OnPropertyChanged(nameof(Errors));
        }
        public override string ToString()
        {
            return LineNumber.ToString();
        }
    }
}
