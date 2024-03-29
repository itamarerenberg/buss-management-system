﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BL.BLApi;
using BLApi;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using PLGui.Models.PO;
using PLGui.utilities;

namespace PLGui.utilities
{
    public class NewStationViewModel : ObservableRecipient
    {
        IBL source;
        private Station station;

        #region properties

        public Station Station
        {
            get => station;
            set => SetProperty(ref station, value);
        }

        public string ButtonCaption { get; set; }
        public bool NewStationMode { get; set; }

        #endregion

        #region constructor

        public NewStationViewModel()
        {
            Station = WeakReferenceMessenger.Default.Send<RequestStation>();//requests the old station (if exist)

            if (station != null)// we are on updateing mode
            {
                NewStationMode = false;
                ButtonCaption = "Update Station"; 
            }
            else                    // we are on new station mode
            {
                station = new Station();
                NewStationMode = true;
                ButtonCaption = "Add Station";
            }

            source = BLFactory.GetBL("admin");

            ButtonCommand = new RelayCommand<Window>(Add_Update_Button);
            CloseCommand = new RelayCommand<Window>(Close);
        }

        #endregion

        #region commands
        public ICommand ButtonCommand { get; }
        public ICommand CloseCommand { get; }

        private void Add_Update_Button(Window window)
        {
            if (NewStationMode == false)//if the view model on "updating mode"
            {
                UpdateStation(window);
            }
            else                        //New Station Mode
            {
                AddStation(window);
            }
        }
        #endregion

        #region Help methods
        BackgroundWorker addStationWorker;
        private void AddStation(Window window)
        {
            if (addStationWorker == null)
            {
                addStationWorker = new BackgroundWorker();
                addStationWorker.DoWork +=
                    (object sender, DoWorkEventArgs args) =>
                    {
                        BackgroundWorker worker = (BackgroundWorker)sender;
                        source.AddStation(station.BOstation);
                    };//this function will execute in the BackgroundWorker thread
                addStationWorker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs arg) =>
                {
                    window.Close();
                };
            }
            addStationWorker.RunWorkerAsync();
        }

        BackgroundWorker updateStationWorker;
        private void UpdateStation(Window window)
        {
            if (updateStationWorker == null)
            {
                updateStationWorker = new BackgroundWorker();
                updateStationWorker.DoWork +=
                    (object sender, DoWorkEventArgs args) =>
                    {
                        source.UpdateStation(station.BOstation);
                    };//this function will execute in the BackgroundWorker thread
                updateStationWorker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs arg) =>
                {
                    window.Close();
                };
            }
            updateStationWorker.RunWorkerAsync();
        }
        private void Close(Window window)
        {
            window.Close();
        }
        #endregion
    }
}
