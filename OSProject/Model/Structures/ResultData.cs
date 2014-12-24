using System;
using System.ComponentModel;

namespace OSProject.Model.Structures
{
    public class ResultData : INotifyPropertyChanged, ICloneable

    {
        private double _consumption;
        public double Consumption
        {
            get { return _consumption; }
            set
            {
                _consumption = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Consumption"));
                }
            }
        }
        private double _levelDeviation;
        public double LevelDeviation
        {
            get { return _levelDeviation; }
            set
            {
                _levelDeviation = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LevelDeviation"));
                }
            }
        }
        private double _pressure;
        public double Pressure
        {
            get { return _pressure; }
            set
            {
                _pressure = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Pressure"));
                }
            }
        }

        public object Clone()
        {
            return new ResultData
            {
                Consumption = this.Consumption,
                LevelDeviation = this.LevelDeviation,
                Pressure = this.Pressure
            };
        }

    public event PropertyChangedEventHandler PropertyChanged;
}
}
