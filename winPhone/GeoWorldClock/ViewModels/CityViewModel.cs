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

namespace GeoWorldClock
{
    public class CityViewModel
    {

        public CityViewModel()
        {
            this.Cities = new ObservableCollection<CityItemViewModel>();
        }

        /// <summary>
        /// A collection for CityViewModel objects.
        /// </summary>
        public ObservableCollection<CityItemViewModel> Cities { get; private set; }

        /// <summary>
        /// convert a coord (lat or long) to double from String
        /// </summary>
        /// <param name="coord">The coord to convert</param>
        /// <returns>The coord converted</returns>
        public double convertCoordToDouble(String coord)
        {
            try
            {
                return Convert.ToDouble(coord);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// load cities from google geolocate service using a cityname criteria
        /// </summary>
        /// <param name="cityName"></param>
        public void LoadCityItems(String cityName)
        {
            WebClient client = new WebClient();

            client.OpenReadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    return;

                Stream str = e.Result;
                XDocument xdoc = XDocument.Load(str);

                var items = (from item in xdoc.Descendants("GeocodeResponse").Descendants("result")
                             select new CityItemViewModel()
                             {
                                 City = prepareCityName(item.Element("formatted_address").Value),
                                 Lat = convertCoordToDouble(item.Element("geometry").Element("location").Element("lat").Value),
                                 Lng = convertCoordToDouble(item.Element("geometry").Element("location").Element("lng").Value)
                             }).ToList();

                // close
                str.Close();         

                // add results to the list
                this.Cities.Clear();
                foreach (CityItemViewModel item in items)
                {
                    this.Cities.Add(item);
                }
            };
            client.OpenReadAsync(new Uri("http://maps.googleapis.com/maps/api/geocode/xml?address=" + cityName + "&sensor=false", UriKind.Absolute));
            
        }

        /// <summary>
        /// Sometimes a city name starts by postal code, if so, we remove it.
        /// </summary>
        /// <param name="cityName">the cityname</param>
        /// <returns>the modified city name</returns>
        private string prepareCityName(string cityName)
        {
            while (isNumeric(cityName.Substring(0,1)))
            {
                cityName = cityName.Substring(1, cityName.Length-1);
            }

            return cityName.Trim();
        }

        /// <summary>
        /// check if a String is numeric or not
        /// </summary>
        /// <param name="val">the String to check if is numeric</param>
        /// <returns>true if val is numeric, false if not</returns>
        private bool isNumeric(string val)
        {
            try
            {
                Convert.ToInt32(val);
                return true;
            }
            catch
            {
                return false;
            }
        }


    }

}
