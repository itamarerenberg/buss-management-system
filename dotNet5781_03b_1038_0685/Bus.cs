﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Internal;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace dotNet5781_03b_1038_0685
{
    public enum StatEnum {READY,IS_TRAVELING,IN_REFUELING,IN_TREATMENT }
    public class Bus : INotifyPropertyChanged
    {
        #region privates fildes
        private string licensNum;
        private StatEnum stat;
        private double fule_in_km;
        private double sumKm;
        private double kmAfterTreat;
        private DateTime lastTreatDate;
        readonly DateTime startDate;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region properties
        [DisplayName("license number")]
        public string LicensNum
        {
            get => this.licensNum;
            set
            {
                if (!value.All(char.IsDigit))//confirm that all characters in value are digites
                {
                    throw new ArgumentException("licensNum must contain only digites");
                }
                if (this.startDate >= new DateTime(2018, 1, 1) && value.Length != 8)//if the StartTime is after or equals 1/1/2018 and the number length is not 8
                {
                    throw new ArgumentException("the length of the licen's num must be suitible to the year of the bus");
                }
                if (this.startDate < new DateTime(2018, 1, 1) && value.Length != 7)//if the StartTime is before 1/1/2018 and the number length is not 7
                {
                    throw new ArgumentException("the length of the licen's num must be suitible to the year of the bus");
                }
                //generates the licens number in format xx-xxx-xx or xxx-xx-xxx
                if (value.Length == 8)
                {
                    value = value.Insert(5, "-");
                    value = value.Insert(3, "-");
                }
                else
                {
                    value = value.Insert(5, "-");
                    value = value.Insert(2, "-");
                }
                this.licensNum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LicensNum"));
            }
        }
        [DisplayName("Start Date")]
        public DateTime StartDate { get => startDate; }
        [DisplayName("Status")]
        public StatEnum Stat { get => stat; 
            set
            {
                stat = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Stat"));
            }
        }
        [DisplayName("fuel status(km)")]
        public double Fule_in_km { get => fule_in_km;
            set 
            {
                fule_in_km = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Fule_in_km"));
            }
        }
        [DisplayName("total km")]
        public double SumKm { get => sumKm;
            set
            {
                sumKm = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SumKm"));
            }
        }
        [DisplayName("km after treatment")]
        public double KmAfterTreat { get => kmAfterTreat;
            set
            {
                kmAfterTreat = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("KmAfterTreat"));
            }
        }
        [DisplayName("Last treatment Date")]
        public DateTime LastTreatDate { get => lastTreatDate;
            set
            {
                lastTreatDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LastTreatDate"));
            }
        }

        #endregion

        #region constractors
        public Bus() { }
        public Bus(string _licensNum, DateTime _startDate, double kmAfterTreat = 0, double sumKm = 0, double _fule_in_km = 1200, DateTime _lastTreatDate = new DateTime(), StatEnum status = 0)
        {
            this.startDate = _startDate;
            this.LicensNum = _licensNum;
            this.KmAfterTreat = kmAfterTreat;
            this.SumKm = sumKm;
            this.Fule_in_km = _fule_in_km;
            this.LastTreatDate = _lastTreatDate;
            Stat = status;
        }

        #endregion

        #region methods

        public void Ride(double km)
        {
            if (this.Stat == StatEnum.IN_REFUELING)
            {
                throw new ArgumentException("the bus is refeuling, please wait!");

            }
            if (this.Stat == StatEnum.IN_TREATMENT)
            {
                throw new ArgumentException("the bus is in treatment, please wait!");

            }
            if (this.Stat == StatEnum.IS_TRAVELING)
            {
                throw new ArgumentException("the bus is already on the ride!");

            }
            //check if the last treatment was less then one year
            if (DateTime.Now - LastTreatDate > new TimeSpan(365, 0, 0, 0))
            {
                throw new ArgumentException("passed more than a year from the last treatment");
            }

            //crheck if this ride will cose to pass the 20,000km from last treatment
            if (KmAfterTreat + km > 20000)
            {
                throw new ArgumentException("the bus need a 20,000's treatment");
            }

            //check if there is enough fule for the ride
            if (Fule_in_km < km)
            {
                throw new ArgumentException("there is not enough fuel for this ride");
            }

            //update the km end the fule
            Fule_in_km -= km;
            KmAfterTreat += km;
            SumKm += km;
            Stat = StatEnum.IS_TRAVELING;
            int time = (int)((km / new Random().Next(20, 50)) * 6000);
            new Thread(() => { Thread.Sleep(time); Stat = StatEnum.READY; }).Start();

        }

        public void Refule()
        {
            if (Stat == StatEnum.READY)
            {
                Stat = StatEnum.IN_REFUELING;
                new Thread(() => { Thread.Sleep(12000); Stat = StatEnum.READY; Fule_in_km = 1200; }).Start(); 
            }
            else
            {
                throw new ArgumentException("you cannot refuel the bus while driving or treatmenting");
            }
        }

        public void Treatment()
        {
            if (stat == StatEnum.READY)
            {
                LastTreatDate = DateTime.Now;
                KmAfterTreat = 0;
                Stat = StatEnum.IN_TREATMENT;
                new Thread(() => { Thread.Sleep(144000); Stat = StatEnum.READY; }).Start(); 
            }
            else
            {
                throw new ArgumentException("you cannot treatment the bus while driving or refueling");
            }
        }

        public override string ToString()
        {
            return $"license's num: {LicensNum},    km: {KmAfterTreat}";
        }

        #endregion
    }
}
