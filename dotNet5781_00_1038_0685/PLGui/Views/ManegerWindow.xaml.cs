﻿
using PLGui.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PLGui
{
    /// <summary>
    /// Interaction logic for Maneger.xaml
    /// </summary>
    public partial class ManegerWindow : Window
    {
        BO.Manager manager;
        public ManegerWindow(BO.Manager _manager)
        {
            InitializeComponent();
            manager = _manager;
            BusesList.DataContext = manager.Buses;
        }

        BackgroundWorker GetManegerInfoWorker;
        internal void blGetManegerInfo()
        {
            if(GetManegerInfoWorker != null)
            {
                GetManegerInfoWorker.CancelAsync();
            }
            GetManegerInfoWorker = new BackgroundWorker();
            GetManegerInfoWorker.WorkerSupportsCancellation = true;
            GetManegerInfoWorker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs args) =>
            {
                if (!((BackgroundWorker)sender).CancellationPending)
                    mngr = (BO.Maneger)args.Result;
            }
        }
    }
}
