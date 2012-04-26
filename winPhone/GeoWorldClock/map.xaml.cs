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
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using System.IO;
using System.Xml.Linq;
using System.Text;
using System.Globalization;

namespace GeoWorldClock
{

    public partial class map : PhoneApplicationPage
    {

        ClockItemViewModel clock = null;

        //Guardaremos aqui el Rect de localización para poder centrar el mapa
        IEnumerable<GeoCoordinate> loc = null;

        //constructor de la pantalla
        public map()
        {
            InitializeComponent();
        }

        //Se ejecuta al cargar la página
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            clock = App.ClockViewModel.Clocks.ElementAt(int.Parse(NavigationContext.QueryString["indexOfCity"]));
            draw_ClockToMap();
        }

        //Evento al pinchar sobre el pin de una oficina
        void pin1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //NavigationService.Navigate(new Uri("/YouMakeThisPage1.xaml", UriKind.Relative));

            //Buscamos la oficina clickada en la lista
            //(sender as Pushpin).Name))
        }

        //Función para abrir el detalle de una oficina
        //void showOfficeInfo(office o)
        //{
        //    (App.Current as App).selectedOffice = o;
        //    NavigationService.Navigate(new Uri("/officeDetails.xaml", UriKind.Relative));
            /*officeDetails root = Application.Current.RootVisual as officeDetails;
            root.o = o;*/
            //MessageBox.Show(o.nombre);

        //}


        //Dibujamos las oficinas descargadas en el Mapa (y el punto donde nos encontramos
        void draw_ClockToMap()
        {

            map1.Children.Clear();

            //map1.Mode = new AerialMode();

            Pushpin pin = new Pushpin();
            pin.Location = new GeoCoordinate(clock.Lat,  clock.Lng);
            pin.MouseLeftButtonUp += new MouseButtonEventHandler(pin1_MouseLeftButtonUp);
            pin.Template = (ControlTemplate)this.Resources["PushpinClock"];
            pin.Name = clock.City;
            map1.Children.Add(pin);

            //Se hace un setView para encuadrar el Mapa. Para esto, se ponen todos los puntos en un IEnumerable y el sistema hace el encuadre solo
            List<GeoCoordinate> l = new List<GeoCoordinate>();
            foreach (Pushpin p in map1.Children)
            {
                l.Add(p.Location);
            }

            loc = l;
            map1.SetView(LocationRect.CreateLocationRect(loc));
            
            map1.ZoomLevel = 8;
        }

    }
}