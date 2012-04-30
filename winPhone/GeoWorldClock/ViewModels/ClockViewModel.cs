using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.IO.IsolatedStorage;
using System.Globalization;
using Microsoft.Phone.Shell;
using System.Device.Location;
using Microsoft.Phone.Net.NetworkInformation;

namespace GeoWorldClock
{
    /// <summary>
    /// All the logic needed to manage the clock API calls and the list management
    /// </summary>
    public class ClockViewModel
    {

        private static System.Windows.Threading.DispatcherTimer _timer = null;
        private string _lastUpdate = ""; //the last time we updated the time
        private Boolean _timerWorking = false;
        private Boolean _loadedData = false;

        public ClockViewModel()
        {
            this.Clocks = new ObservableCollection<ClockItemViewModel>();
        }

        /// <summary>
        /// A collection for ClockViewModel objects.
        /// </summary>
        public ObservableCollection<ClockItemViewModel> Clocks { get; private set; }

        /// <summary>
        /// search a lat & long in the geonames.org API to get the timezone, and then, if the clock is not in the list, add to it.
        /// saveToDisk after adding clock
        /// </summary>
        /// <param name="cityName">city name</param>
        /// <param name="lat">latitude of the city</param>
        /// <param name="lng">longitude of the city</param>
        public void addClock(String cityName, double lat, double lng)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("You need internet connection to request timezone information. Please, chack your internet connetion and try again.");
                return;
            }

            if (Clocks.Count >= 6)
            {
                MessageBox.Show("You have reached the maximun number of clocks available.");
                return;
            }

            WebClient client = new WebClient();

            client.OpenReadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    return;

                Stream str = e.Result;
                XDocument xdoc = XDocument.Load(str);

                CultureInfo ci = new CultureInfo(CultureInfo.CurrentCulture.Name);

                //create the Clock element
                ClockItemViewModel c = CreateClockViewModel(                             
                                 cityName,
                                 lat,
                                 lng,
                                 Convert.ToDouble(xdoc.Element("geonames").Element("timezone").Element("dstOffset").Value.Replace(",", ci.NumberFormat.CurrencyDecimalSeparator).Replace(".", ci.NumberFormat.CurrencyDecimalSeparator))
                             );

                //add to list if not found
                if (!isClockInList(c))
                {
                    Clocks.Add(c);
                    saveToDisk();
                }
                else
                {
                    MessageBox.Show(c.City + " is already on the list.");
                }                

                str.Close();

                SystemTray.IsVisible = false;
            };
            SystemTray.IsVisible = true;
            String uri = "http://api.geonames.org/timezone?lat=" + lat + "&lng=" + lng + "&username=grjordi";
            uri = uri.Replace(',','.');
            client.OpenReadAsync(new Uri(uri));
            
        }

        /// <summary>
        /// remove a clock from the list if exists
        /// savetoDisk after
        /// </summary>
        /// <param name="cityName"></param>
        public void remove(string cityName)
        {
            bool found = false;
            int i = 0;
            for (i=0; i<Clocks.Count; i++)
            {
                if ((Clocks[i] as ClockItemViewModel).City == cityName)
                {
                    found=true;
                    break;
                }
                
            }

            if (found) Clocks.RemoveAt(i);

            saveToDisk();
        }

        /// <summary>
        /// remove a clock from the list if exists
        /// savetoDisk after
        /// </summary>
        /// <param name="cityName"></param>
        public int indexOf(string cityName)
        {
            bool found = false;
            int i = 0;
            for (i = 0; i < Clocks.Count; i++)
            {
                if ((Clocks[i] as ClockItemViewModel).City == cityName)
                {
                    found = true;
                    break;
                }

            }

            if (found) return i;
            else return -1;

        }


        /// <summary>
        /// remove a clock from the list if exists
        /// savetoDisk after
        /// </summary>
        /// <param name="cityName"></param>
        public GeoCoordinate getCoordinate(string cityName)
        {
            GeoCoordinate g = new GeoCoordinate(0, 0);
            bool found = false;
            int i = 0;
            for (i = 0; i < Clocks.Count; i++)
            {
                if ((Clocks[i] as ClockItemViewModel).City == cityName)
                {
                    found = true;
                    break;
                }

            }

            if (found) g = new GeoCoordinate(Clocks[i].Lat, Clocks[i].Lng);

            return g;
        }


        /// <summary>
        /// Update the time of all the clocks. Must check if it's necessary before call.
        /// </summary>
        public void UpdateTimes()
        {
            //for the moment, I make this by deleting all the elements and create it again. Need to found something more elegant.

            //copyOldClocks
            ObservableCollection<ClockItemViewModel> oldClocks = new ObservableCollection<ClockItemViewModel>();
            foreach (ClockItemViewModel item in Clocks)
                oldClocks.Add(item);

            emptyClockList();

            //add again
            foreach (ClockItemViewModel item in oldClocks)          
                Clocks.Add(CreateClockViewModel(item.City, item.Lat, item.Lng, item.GmtOffset));                          
        }

        /// <summary>
        /// empty the clocks list
        /// </summary>
        public void emptyClockList()
        {
            //removeFromList
            while (Clocks.Count > 0)
                Clocks.RemoveAt(0);
        }


        /// <summary>
        /// search for a clock in the clock list
        /// </summary>
        /// <param name="c">ClockItemViewModel to find</param>
        /// <returns></returns>
        public Boolean isClockInList(ClockItemViewModel c)
        {
            foreach (ClockItemViewModel item in Clocks)
                if (item.City == c.City) return true;

            return false;
        }


        /// <summary>
        /// create a ClockItemViewModel with the correct time String setted
        /// </summary>
        /// <param name="cityName">city name</param>
        /// <param name="lat">latitude of the city</param>
        /// <param name="lng">longitude of the city</param>
        /// <param name="GmtOffset">The offset from UTC time. Can be positive or negative</param>
        /// <returns>a ClockItemViewModel correctly created and filled with data</returns>
        public ClockItemViewModel CreateClockViewModel(String cityName, double lat, double lng, double GmtOffset)
        {
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.ToUniversalTime();
            dateTime = dateTime.Add(TimeSpan.FromHours(GmtOffset));
            string timeStr = dateTime.ToString("H:mm");
            return new ClockItemViewModel() { City = cityName, GmtOffset = GmtOffset, Lat = lat, Lng = lng, Time = timeStr };
        }

        /// <summary>
        /// start the timer to update the times
        /// </summary>
        public void startTimer()
        {
            if (!_loadedData) loadFromDisk();

            if (_timerWorking) return;

            UpdateTimes();
            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1000); // 500 Milliseconds
            _timer.Tick += new EventHandler(dt_Tick);
            _timer.Start();
            _timerWorking = true;
        }


        /// <summary>
        /// stop the timer for updating times
        /// </summary>
        public void stopTimer()
        {
            if (!_timerWorking) return;


            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// The event fired by the timer.
        /// if the lastUpdate is diferent than the time calculated now, we call updateTimes();
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void dt_Tick(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            if (_lastUpdate != dateTime.ToString("hh:mm"))
            {
                _lastUpdate = dateTime.ToString("hh:mm");
                UpdateTimes();
            }
            
        }

        /// <summary>
        /// save the clock list to disk
        /// </summary>
        public void saveToDisk()
        {
            IsolatedStorageFile store;
            IsolatedStorageFileStream timefile;

            store = IsolatedStorageFile.GetUserStoreForApplication();

            if (!store.FileExists("clocks.txt"))
            {
                timefile = store.OpenFile("clocks.txt", FileMode.Create, FileAccess.Write);
            }
            else
            {
                timefile = store.OpenFile("clocks.txt", FileMode.Create, FileAccess.Write);
            }

            using (StreamWriter writer = new StreamWriter(timefile))
            {
                foreach (ClockItemViewModel item in Clocks)
                    writer.WriteLine(item.City + ";" + item.Lat + ";" + item.Lng + ";" + item.GmtOffset);

                writer.Close();
            }

        }

        /// <summary>
        /// load the clocks from disk
        /// </summary>
        public void loadFromDisk()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream timefile = store.OpenFile("clocks.txt", FileMode.OpenOrCreate, FileAccess.Read);

            string lines;

            emptyClockList();

            using (StreamReader reader = new StreamReader(timefile))
            {
                while ((lines = reader.ReadLine()) != null)
                {
                    var clockStringModel = lines.Split(';');
                    if (clockStringModel.Length==4)
                        Clocks.Add(CreateClockViewModel(clockStringModel[0], double.Parse(clockStringModel[1]), double.Parse(clockStringModel[2]), double.Parse(clockStringModel[3])));
                }
            }

            _loadedData = true;

        
        }

    }

}
