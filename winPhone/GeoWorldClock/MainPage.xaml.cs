using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace GeoWorldClock
{
    public partial class MainPage : PhoneApplicationPage
    {

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            this.searchListBox.DataContext = App.CityViewModel;
            this.clockListBox.DataContext = App.ClockViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.ClockViewModel.startTimer();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = sender as TextBox;

            App.CityViewModel.LoadCityItems(t.Text);
        }

        private void searchListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ListBox l = sender as ListBox;

            if (l.SelectedIndex >= 0)
            {
                CityItemViewModel c = l.SelectedItem as CityItemViewModel;

                if (c != null)
                {
                    App.ClockViewModel.addClock(c.City, c.Lat, c.Lng);
                }
            }
        }

        private void MenuItem_DeleteClockClick(object sender, RoutedEventArgs e)
        {
            MenuItem itm = (sender as MenuItem);

            App.ClockViewModel.remove(itm.CommandParameter.ToString());
        }

        public void delete(String city)
        {

        }
    }
}