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
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

namespace GeoWorldClock
{
    public partial class MainPage : PhoneApplicationPage
    {

        public ImageBrush PanoramaBackgroundImage
        {
            get
            {
                var lightThemeEnabled = (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"] == Visibility.Visible;

                var url = lightThemeEnabled ? "worldmap_white.png" : "worldmap.png";

                var brush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute))
                };
                return brush;
            }
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.panoramaBackgroundImage.ImageSource = PanoramaBackgroundImage.ImageSource;

            // Set the data context of the listbox control to the sample data
            this.searchListBox.DataContext = App.CityViewModel;
            this.clockListBox.DataContext = App.ClockViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            //hide or show info panel
            if (App.ClockViewModel.Clocks.Count <= 0) infoPanel.Visibility = System.Windows.Visibility.Visible;
            else infoPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.ClockViewModel.startTimer();
            //hide or show info panel
            if (App.ClockViewModel.Clocks.Count <= 0) infoPanel.Visibility = System.Windows.Visibility.Visible;
            else infoPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// event when the user changes the text of the searchTextBox.
        /// When it happens, we load new cities into the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = sender as TextBox;

            App.CityViewModel.LoadCityItems(t.Text);
        }

        /// <summary>
        /// happens when the user selects one city into the city searched list
        /// Adds a new clock to the clockPanel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ListBox l = sender as ListBox;

            if (l.SelectedIndex >= 0)
            {
                CityItemViewModel c = l.SelectedItem as CityItemViewModel;

                if (c != null)
                {
                    App.ClockViewModel.addClock(c.City, c.Lat, c.Lng);

                    //hide info panel
                    infoPanel.Visibility = System.Windows.Visibility.Collapsed;

                    //move to default panorama and delete search results
                    mainPanorama.DefaultItem = mainPanorama.Items[0];
                    mainPanorama.Focus();
                    searchTextBox.Text = "";
                    App.CityViewModel.Cities.Clear();
                }
            }
        }

        /// <summary>
        /// Fired when the user delete a clock from the contextmenu
        /// Remove a clock from the clockpanel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_DeleteClockClick(object sender, RoutedEventArgs e)
        {
            MenuItem itm = (sender as MenuItem);

            App.ClockViewModel.remove(itm.CommandParameter.ToString());

            //hide or show info panel
            if (App.ClockViewModel.Clocks.Count <= 0) infoPanel.Visibility = System.Windows.Visibility.Visible;
            else infoPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Action when first panorama explanation button is pressed
        /// Go to the search panorama item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //move to the search panorama
            mainPanorama.DefaultItem = mainPanorama.Items[1];
            searchTextBox.Focus();
        }

        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StackPanel st = sender as StackPanel;

            TextBlock txCityName = st.Children[1] as TextBlock;
            int i = App.ClockViewModel.indexOf(txCityName.Text);

            NavigationService.Navigate(new Uri("/map.xaml?indexOfCity=" + i, UriKind.Relative));

        }



    }
}