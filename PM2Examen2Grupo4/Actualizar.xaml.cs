using MediaManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2Examen2Grupo4
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Actualizar : ContentPage
    {
        int idS;
        public Actualizar(string actualizar, string video, int Id)
        {
            InitializeComponent();
            txtdescripcion.Text = actualizar;
            Task.Run(() => CrossMediaManager.Current.Stop());
            CrossMediaManager.Current.Play();
            CrossMediaManager.Current.Speed = 1F;
            videov.Source = video;
            idS = Id;
        }

        private void BtnActu_Clicked(object sender, EventArgs e)
        {
            try
            {
                WebClient cliente = new WebClient();

                var parametros = new System.Collections.Specialized.NameValueCollection();
                parametros.Add("Descripcion", txtdescripcion.Text);
                cliente.UploadValues("http://192.168.1.37/Examen2PMovil2/Actualizar.php?Id=" + idS, "POST", parametros);
                DisplayAlert("Actualizar", "Actualizado Correctamente", "Ok");
                Lista pagina = new Lista();
                Navigation.PushAsync(pagina);
            }
            catch (WebException ex)
            {
                // Manejar la excepción adecuadamente
                Console.WriteLine("Error al actualizar el sitio: " + ex.Message);
            }

        }
    }
}