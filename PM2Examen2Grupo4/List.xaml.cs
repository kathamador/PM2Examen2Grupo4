using MediaManager;
using Plugin.AudioRecorder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2Examen2Grupo4
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class List : ContentPage
    {
        private readonly AudioPlayer audioPlayer = new AudioPlayer();
        string ruta;
        public List(string rutaVideo, string rutaAudio)
        {
            InitializeComponent();
            Task.Run(() => CrossMediaManager.Current.Stop());
            CrossMediaManager.Current.Play();
            CrossMediaManager.Current.Speed = 1F;

            videov.Source = rutaVideo;
            ruta = rutaAudio;
        }

        private void btnescucharAudio_Clicked(object sender, EventArgs e)
        {
            audioPlayer.Play(ruta);
        }
    }
}