﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    public class NewLineViewModel : ObservableRecipient
    {

        #region fields
        IBL source;
        BackgroundWorker loadDataWorker;
        BackgroundWorker NewLineWorker;

        Line tempLine = new Line();
        private bool addManually;

        #endregion

        #region properties

        public Line TempLine
        {
            get => tempLine;
            set
            {
                SetProperty(ref tempLine, value);
            }
        }
        public bool AddManually
        {
            get => addManually;
            set
            {
                SetProperty(ref addManually, value);
            }
        }
        public string buttonCaption { get; set; }
        public bool NewLineMode{ get; set; }
        public bool IsMinStation { get => Stations.Count >= 2; }
        public bool Processing { get; set; } = true;

        public ObservableCollection<Station> DBStations { get; set; }//data base stations
        public ObservableCollection<LineStation> Stations { get; set; } = new ObservableCollection<LineStation>();//new/updated line stations
        public List<string> ComboList { get; set; } = new List<string>() { "Name", "Code", "Address" };


        #endregion

        #region constructors
        /// <summary>
        /// view model for generating/updating line. the constructor send a message to the sender
        /// to determaine the mode of the view model(generating a new line or updating an existing one)
        /// </summary>
        public NewLineViewModel()
        {

            tempLine = WeakReferenceMessenger.Default.Send<RequestLine>();//requests the old line (if exist)

            if (tempLine != null)// we are in update line mode
            {
                NewLineMode = false;
                buttonCaption = "Update line";
            }
            else                // we are in new line mode
            {
                NewLineMode = true;
                buttonCaption = "Add line";
                tempLine = new Line();
            }
            
            //load data
            source = BLFactory.GetBL("admin");
            loadData();

            //commands initalizing
            SelectDBStationCommand = new RelayCommand<object>(SelectDBStation);
            AddLineButton = new RelayCommand<Window>(Add_Update_LineButton);
            DeleteStationCommand = new RelayCommand<object>(DeleteStation);
            SearchCommand = new RelayCommand<object>(SearchBox_TextChanged);
            LoadedCommand = new RelayCommand<NewLineView>(Loaded);
            CloseCommand = new RelayCommand<Window>(Close);
        }


        #endregion

        #region load data
        private void loadData()
        {
            if (loadDataWorker == null)
            {
                loadDataWorker = new BackgroundWorker();
            }
            loadDataWorker.RunWorkerCompleted +=
                (object sender, RunWorkerCompletedEventArgs args) =>
                {
                    if (!((BackgroundWorker)sender).CancellationPending)//if the BackgroundWorker didn't 
                    {                                                   //terminated before he done execute DoWork
                        DBStations = (ObservableCollection<Station>)args.Result;
                        if (NewLineMode == false)//if the view model on "updating mode"
                        {
                            getDataOfOldLine(DBStations, Stations, TempLine);
                            OnPropertyChanged(nameof(Stations));
                            OnPropertyChanged(nameof(IsMinStation));
                        }
                        OnPropertyChanged(nameof(DBStations));
                    }
                };//this function will execute in the main thred

            loadDataWorker.DoWork +=
                (object sender, DoWorkEventArgs args) =>
                {
                    BackgroundWorker worker = (BackgroundWorker)sender;
                    ObservableCollection<Station> result = new ObservableCollection<Station>();
                    result = new ObservableCollection<Station>(source.GetAllStations().Select(st => new Station() { BOstation = st }));
                    args.Result = worker.CancellationPending ? null : result;
                };//this function will execute in the BackgroundWorker thread
            loadDataWorker.RunWorkerAsync();
        }
        #endregion

        #region commands
        public ICommand SelectDBStationCommand { get; }
        public ICommand DeleteStationCommand { get; }
        public ICommand AddLineButton { get; }
        public ICommand SearchCommand { get; }
        public ICommand LoadedCommand { get; }
        public ICommand CloseCommand { get; }

        /// <summary>
        /// double click on data base station, the station will be added to the new/updated line stations
        /// </summary>
        private void SelectDBStation(object sender)
        {
            if ((sender as ListView).SelectedItem is Station selectedStation)
            {
                if (Stations.Count > 0)
                    Stations.Last().NotLast = true;//now the previus last station is no longer the last
                Stations.Add(new LineStation() { Station = selectedStation, NotLast = false});//this is the last station in Stations
                DBStations.Remove(selectedStation);
                OnPropertyChanged(nameof(IsMinStation));

                NewLineView Lview = (sender as Control).FindWindowOfType<NewLineView>();
                SearchBox_TextChanged(Lview);
            }
        }
        /// <summary>
        /// double click on the new/updated line station, the station will be deleted from the stations list
        /// </summary>
        private void DeleteStation(object sender)
        {
            if ((sender as ListView).SelectedItem is LineStation selectedStation)
            {
                int index = (sender as ListView).SelectedIndex;

                DBStations.Add(selectedStation.Station);
                Stations.Remove(selectedStation);
                if (selectedStation.NotLast == false && Stations.Count > 0)//if the selectet station was the last station
                    Stations.Last().NotLast = false;//set the new last station to not NotLast
                OnPropertyChanged(nameof(IsMinStation));

                NewLineView Lview = (sender as Control).FindWindowOfType<NewLineView>();
                SearchBox_TextChanged(Lview);
            }
        }
        /// <summary>
        /// save the changes of distance/time
        /// </summary>
        private void Add_Update_LineButton(Window window)
        {
            //open the dialog and disabled the elements in the window
            (window as NewLineView).DialogProcess.IsOpen = true;
            Processing = false;
            OnPropertyChanged(nameof(Processing));

            if (NewLineMode)
            {
                NewLine(window);
            }
            else//if the view model on "updating mode"
            {
                UpdateLine(window);
            }
        }

        private void NewLine(Window window)
        {
            if (NewLineWorker == null)
            {
                NewLineWorker = new BackgroundWorker();
                NewLineWorker.RunWorkerCompleted +=
                    (object sender, RunWorkerCompletedEventArgs e) =>
                    {
                        (e.Result as Window).Close();
                    };
                NewLineWorker.DoWork +=
                    (object sender, DoWorkEventArgs args) =>
                    {
                        BO.Line BOline = new BO.Line()//creating new BO line
                        {
                            LineNumber = (int)TempLine.LineNumber,
                            Area = (BO.AreasEnum)TempLine.Area
                        };
                        List<int?> distances = Stations.Select(lst => lst.Distance).ToList();
                        List<int?> times = Stations.Select(lst => lst.Time).ToList();
                        try
                        {
                            source.AddLine(BOline, Stations.Select(st => st.Station.BOstation), distances, times);
                            args.Result = args.Argument;
                        }
                        catch (Exception msg)
                        {
                            MessageBox.Show(msg.Message, "ERROR");

                            //return the window to the normal mode
                            (args.Argument as NewLineView).DialogProcess.IsOpen = false;
                            Processing = true;
                            OnPropertyChanged(nameof(Processing));
                        }  
                    };//this function will execute in the BackgroundWorker thread
            }
            NewLineWorker.RunWorkerAsync(window);
        }


        BackgroundWorker updateLineWorker;
        private void UpdateLine(Window window)
        {
            if (updateLineWorker == null)
            {
                updateLineWorker = new BackgroundWorker();
                updateLineWorker.RunWorkerCompleted +=
                    (object sender, RunWorkerCompletedEventArgs e) =>
                    {
                        (e.Result as Window).Close();
                    };
                updateLineWorker.DoWork +=
                    (object sender, DoWorkEventArgs args) =>
                    {
                        BackgroundWorker worker = (BackgroundWorker)sender;
                        List<int?> distances = Stations.Select(lst => lst.Distance).ToList();
                        List<int?> times = Stations.Select(lst => lst.Time).ToList();
                        try
                        {
                            source.UpdateLine((int)TempLine.ID, Stations.Select(st => st.Station.BOstation), distances, times);
                            args.Result = args.Argument;
                        }
                        catch (Exception msg)
                        {
                            MessageBox.Show(msg.Message, "ERROR");

                            //return the window to the normal mode
                            (args.Argument as NewLineView).DialogProcess.IsOpen = false;
                            Processing = true;
                            OnPropertyChanged(nameof(Processing));
                        }
                    };//this function will execute in the BackgroundWorker thread
            }
            updateLineWorker.RunWorkerAsync(window);
        }

        /// <summary>
        /// accured when search box is changing. replace the list in the window into list that contains the search box text.
        /// the search is according to the combo box picking
        /// </summary>
        private void SearchBox_TextChanged(object sender)
        {
            NewLineView Lview = new NewLineView();
            if (sender is TextBox)
            {
                //get the ManegerView instance
                Lview = (((((sender as TextBox).Parent as StackPanel).Parent) as Grid).Parent) as NewLineView;
            }
            else if (sender is NewLineView)
            {
                Lview = sender as NewLineView;
            }
            if (Lview.ComboBoxSearch.SelectedItem != null)
            {
                //get the current presented List, get his name, and create a copy collection for searching
                var tempList = DBStations;
                if (tempList != null)
                {
                    //return a new collection according to the searching letters
                    Lview.DBStationList.ItemsSource = tempList.Where(c => c.GetType().GetProperty(Lview.ComboBoxSearch.Text).GetValue(c, null).ToString().Contains(Lview.SearchBox.Text));
                }
            }
        }

        private void Loaded(NewLineView Lview)
        {
            Lview.LNum.Focus();
            Lview.LNum.SelectAll();
        }
        private void Close(Window window)
        {
            window.Close();
        }
        #endregion

        #region help methods
        /// <summary>
        /// help method for "updating mode". initalize the stations from the line's and data base's stations. 
        /// </summary>
        /// <param name="DBstations">data base stations</param>
        /// <param name="stations">empty collection<LineStation></LineStation></param>
        /// <param name="line">the old line</param>
        private void getDataOfOldLine(Collection<Station> DBstations, Collection<LineStation> stations, Line line )
        {
            foreach (var doLS in line.Stations)
            {
                LineStation lineStation = new LineStation();
                lineStation.Station = DBStations.Where(s => s.Code == doLS.StationNumber).FirstOrDefault();//get the station from the data base
                if (lineStation.Station != null)//if the station is exist get the rest details from the old line's station
                {
                    if (doLS.CurrentToNext != null)
                    {
                        lineStation.Distance = (int?)doLS.CurrentToNext.Distance;
                        lineStation.Time = (int?)doLS.CurrentToNext.Time.TotalMinutes; 
                    }
                    lineStation.Index = doLS.LineStationIndex;
                    lineStation.NotLast = true;
                    stations.Add(lineStation);
                    DBStations.Remove(lineStation.Station);//remove the station from the data base list
                }
            }
            stations.OrderBy(l => l.Index);
            stations.Last().NotLast = false;

            tempLine.LineNumber = line.LineNumber;
            tempLine.Area = line.Area;
        }

        #endregion
    }
}
