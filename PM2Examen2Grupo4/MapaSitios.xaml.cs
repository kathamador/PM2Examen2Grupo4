using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using Plugin.Geolocator;
using Xamarin.Essentials;
using Xamarin.Forms.PlatformConfiguration;
using static Xamarin.Essentials.Permissions;
using System.Diagnostics;
using Map = Xamarin.Essentials.Map;

namespace PM2Examen2Grupo4
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MapaSitios : ContentPage
	{
        public string ubicacion;
		public MapaSitios (double latitud, double longitud,string ubic)
		{
			InitializeComponent ();
            mtxtLat.Text = latitud.ToString ();
            mtxtLon.Text = longitud.ToString ();
            ubicacion = ubic;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            double Latitud = Convert.ToDouble(mtxtLat.Text);
            double Longitud = Convert.ToDouble(mtxtLon.Text);

            var mapac = new Position(Latitud, Longitud);
            var pin = new Pin
            {
                Position = mapac,
                Label = "Ubicación",
                Address = ubicacion
            };

            Mapa.Pins.Add(pin);
            Mapa.MoveToRegion(MapSpan.FromCenterAndRadius(mapac, Distance.FromMiles(1)));

            var localizacion = CrossGeolocator.Current;

            if (localizacion != null)
            {
                localizacion.PositionChanged += Locatilazion_PositionChanged;

                if (!localizacion.IsListening)
                {
                    Debug.WriteLine("StartListeningAsync");
                    localizacion.StartListeningAsync(TimeSpan.FromSeconds(10), 100);
                }
            }
            else
            {
                var mapacc = new Position(Latitud, Longitud);
                Mapa.MoveToRegion(new MapSpan(mapacc, 2, 2));
            }
        }

        private void Locatilazion_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            double Latitud = Convert.ToDouble(mtxtLat.Text);
            double Longitud = Convert.ToDouble(mtxtLon.Text);
            var mapac = new Position(Latitud, Longitud);
            Mapa.MoveToRegion(new MapSpan(mapac, 2, 2));
        }

        private void btnVerM_Clicked(object sender, EventArgs e)
        {
            double latitud = Convert.ToDouble(mtxtLat.Text); // Obtén la latitud del mapa
            double longitud = Convert.ToDouble(mtxtLon.Text); // Obtén la longitud del mapa

            var location = new Location(latitud, longitud);
            var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };

            Map.OpenAsync(location, options);
        }
    }

}
