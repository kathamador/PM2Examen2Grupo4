using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Globalization;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;
using System.Reflection;

namespace PM2Examen2Grupo4
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Lista : ContentPage
    {
        public List<sitios> Datos { get; set; }
        public Lista()
        {
            BindingContext = this;
            InitializeComponent();
            Datos = new List<sitios>();
            listViewSitios.ItemsSource = Datos;
            CargarSitios();
        }


        private async void CargarSitios()
        {
            WebClient client = new WebClient();

            try
            {
                string response = await client.DownloadStringTaskAsync("http://192.168.1.37/Examen2PMovil2/MostrarSitio.php");

                // Parsear la respuesta JSON a una lista de objetos sitios
                var sitiosList = JsonConvert.DeserializeObject<List<sitios>>(response);

                // Asignar la lista de sitios al origen de datos del ListView
                listViewSitios.ItemsSource = sitiosList;
            }
            catch (WebException ex)
            {
                // Manejar la excepción adecuadamente
                Console.WriteLine("Error al obtener los datos: " + ex.Message);
            }
        }

        private async void ListViewSitios_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is sitios selectedItem)
            {
                string rutaVideo = selectedItem.VideoDigital;
                string rutaA = selectedItem.AudioFile;
                string descrip = selectedItem.Descripcion;
                int Id = selectedItem.Id;
                double latitud = selectedItem.Latitud;
                double longitud = selectedItem.Longitud;
                string action = await DisplayActionSheet("Opciones", "Cancelar", null, "Ver Video | Escuchar Audio", "Ver Mapa", "Actualizar", "Eliminar");

                switch (action)
                {
                    case "Ver Mapa":        
                        MapaSitios pagina = new MapaSitios(latitud,longitud,descrip);
                                //pagina.BindingContext = selectedSiti;
                                await Navigation.PushAsync(pagina);
                        break;
                    case "Eliminar":                       
                        // Obtener el objeto seleccionado del ListView
                        sitios selectedSitio = (sitios)e.Item;
                        // Obtener el ID del objeto seleccionado
                        int selectedIdString = selectedSitio.Id;
                      
                            // Llamar al script PHP de eliminación y enviar el ID
                            WebClient client = new WebClient();
                            string url = $"http://192.168.1.37/Examen2PMovil2/Eliminar.php?Id="+selectedIdString;                 
                        try
                            {
                                string response = await client.DownloadStringTaskAsync(url);
                                await DisplayAlert("Eliminado", "Eliminado Exitosamente", "Ok");
                            CargarSitios();
                            }
                            catch (WebException ex)
                            {
                                // Manejar la excepción adecuadamente
                                Console.WriteLine("Error al eliminar el sitio: " + ex.Message);
                            }
                        break;
                    case "Ver Video | Escuchar Audio":
                        List paginaVideo = new List(rutaVideo,rutaA);
                        await Navigation.PushAsync(paginaVideo);
                        break;
                    case "Actualizar":
                        Actualizar actualizar = new Actualizar(descrip,rutaVideo,Id);
                        await Navigation.PushAsync(actualizar);
                        break;
                    default:
                        // Cancelar o acción desconocida
                        break;
                }
            }
        }
    }
}