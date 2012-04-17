﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GeoWorldClock
{
    public class ClockItemViewModel
    {
        private string _city;
        private double _lat;
        private double _lng;
        private double _gmtOffset;
        private string _time;

        public string City
        {
            get
            {
                return _city;
            }
            set
            {
                _city = value;
            }
        }

        public double Lat
        {
            get
            {
                return _lat;
            }
            set
            {
                _lat = value;
            }
        }

        public double Lng
        {
            get
            {
                return _lng;
            }
            set
            {
                _lng = value;
            }
        }

        public double GmtOffset
        {
            get
            {
                return _gmtOffset;
            }
            set
            {
                _gmtOffset = value;
            }
        }

        public string Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
            }
        }

    }
}
