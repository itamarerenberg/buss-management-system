﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLApi;
using PLGui.Models;

namespace PLGui.ViewModels
{
    public class ManegerViewModel : INotifyPropertyChanged
    {
        ManegerModel model;
        public event PropertyChangedEventHandler PropertyChanged;
        IBL source;

        #region properties

        public ObservableCollection<BO.Bus> Buses
        {
            get => model.Buses;
            set
            {
                model.Buses = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Lines"));
            }
        }

        public ObservableCollection<BO.Line> Lines
        {
            get => model.Lines;
            set
            {
                model.Lines = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Lines"));
            }
        }

        #endregion

        #region constractors

        public ManegerViewModel(IBL bl)
        {
            loadData();
            source = bl;
        }

        BackgroundWorker load_data;
        private void loadData()
        {
            if(load_data == null)
            {
                load_data = new BackgroundWorker();
            }

            load_data.RunWorkerCompleted +=
                (object sender, RunWorkerCompletedEventArgs args) =>
                {
                    if (!((BackgroundWorker)sender).CancellationPending)//if the BackgroundWorker didn't 
                    {                                                   //terminated befor he done execute DoWork
                        model = (ManegerModel)args.Result;
                    }
                };//this function will execute in the main thred

            load_data.DoWork +=
                (object sender, DoWorkEventArgs args) =>
                {
                    BackgroundWorker worker = (BackgroundWorker)sender;
                    ManegerModel result = new ManegerModel();
                    result.Buses = (ObservableCollection<BO.Bus>)source.GetAllBuses();//!possible problem: ther is no conversion from IEnumerable to ObservableColection
                    result.Lines = (ObservableCollection<BO.Line>)source.GetAllLines();//same⬆
                    args.Result = worker.CancellationPending ? null : result;
                };//this function will execute in the BackgroundWorker thread
        }

        #endregion
    }
}