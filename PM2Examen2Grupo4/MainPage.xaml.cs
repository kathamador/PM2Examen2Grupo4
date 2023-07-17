using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xam.Forms.VideoPlayer;
using Plugin.Geolocator;
using System.Reflection;
using System.IO;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using Plugin.AudioRecorder;
using Xamarin.Essentials;
using System.Net.Http;
using System.Net;
using NetworkAccess = Xamarin.Essentials.NetworkAccess;

namespace PM2Examen2Grupo4
{
    public partial class MainPage : ContentPage
    {
        public string PhotoPath;

        private readonly AudioRecorderService audioRecorderService = new AudioRecorderService();
        private readonly AudioPlayer audioPlayer = new AudioPlayer();
        public string AudioPath, fileName;

        public bool takedvideo = false;
        public bool audio = false;

        public bool takedfoto = false;
        public byte[] imgbyte;
        public Image image = new Image();

        private bool videoGrabado = false;
        private bool audioGrabado = false;

        public MainPage()
        {
            InitializeComponent();
            Conexion();
        }

        //primera validacion de si tiene internet el usuario
        private async void Conexion()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Sin Internet", "No hay conexion a internet", "Ok");
                return;
            }
            else
            {
                await DisplayAlert("Conectado", "Conexion a Internet Estable", "Ok");
                LoadCoord();
            }
        }

        //llamamos el metodo grabar video
        private async void BtnGrabarVideo_Clicked(object sender, EventArgs e)
        {
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                return;
            }

            GrabarVideo();
        }

        //este metodo es para grabar el video
        public async void GrabarVideo()
        {
            try
            {
                var photo = await MediaPicker.CaptureVideoAsync();
                await LoadPhotoAsync(photo);
                Console.WriteLine($"CapturePhotoAsync COMPLETED: {PhotoPath}");

                videoGrabado = true;

                UriVideoSource uriVideoSurce = new UriVideoSource()
                {
                    Uri = PhotoPath
                };

                videoP.Source = uriVideoSurce;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        async Task LoadPhotoAsync(FileResult photo)
        {
            if (photo == null)
            {
                PhotoPath = null;
                return;
            }

            var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            {
                byte[] data = new byte[stream.Length];
                await stream.ReadAsync(data, 0, (int)stream.Length);
                File.WriteAllBytes(newFile, data);
            }
            PhotoPath = newFile;
        }

        private async void LoadCoord()
        {
            try
            {
                var georequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var tokendecancelacion = new System.Threading.CancellationTokenSource();

                // Solicitar permisos de ubicación
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status == PermissionStatus.Granted)
                {
                    var location = await Geolocation.GetLocationAsync(georequest, tokendecancelacion.Token);
                    if (location != null)
                    {
                        var lon = location.Longitude;
                        var lat = location.Latitude;

                        txtlatitud.Text = lat.ToString();
                        txtlongitud.Text = lon.ToString();
                    }
                    else
                    {
                        await DisplayAlert("Advertencia", "No se pudo obtener la ubicación", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Advertencia", "Sin permisos de ubicación", "OK");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Advertencia", "Este dispositivo no soporta GPS" + fnsEx, "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Advertencia", "Error de Dispositivo, Validar si su GPS esta activo", "OK");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Advertencia", "Sin Permisos de Ubicacion" + pEx, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Advertencia", "Sin Ubicacion " + ex, "OK");
            }
        }

        void ShowAlertMessage(string message)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Alerta", message, "OK");
            });
        }

        private async void BtnGrabarAudio_Clicked(object sender, EventArgs e)
        {
            var status = await Permissions.RequestAsync<Permissions.Microphone>();
            var status2 = await Permissions.RequestAsync<Permissions.StorageRead>();
            var status3 = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted || status2 != PermissionStatus.Granted || status3 != PermissionStatus.Granted)
            {
                return; // si no tiene los permisos no avanza
            }

            audio = true;

            if (string.IsNullOrEmpty(txtdescripcion.Text))
            {
                await DisplayAlert("Alerta", "Ingrese una descripción", "OK");
                return;
            }


            if (audioRecorderService.IsRecording)
            {
                await audioRecorderService.StopRecording();
                audioPlayer.Play(audioRecorderService.GetAudioFilePath());
                audioGrabado = true;

            }
            else
            {
                btnGrabarAudio.IsEnabled = false;
                btnDetener.IsEnabled = true;
                await audioRecorderService.StartRecording();
            }
        }

        private async void BtnDetener_Clicked(object sender, EventArgs e)
        {
            if (audioRecorderService.IsRecording)
            {
                await audioRecorderService.StopRecording();
                SaveAudio();

                if (await DisplayAlert("Audio", "¿Desea reproducir el audio?", "SI", "NO"))
                {
                    audioPlayer.Play(audioRecorderService.GetAudioFilePath());
                }
                btnGrabarAudio.IsEnabled = true;
                btnDetener.IsEnabled = false;

                audioGrabado = true;
            }
            else
            {
                await audioRecorderService.StartRecording();
            }
        }

        private void SaveAudio()
        {
            if (audioRecorderService.FilePath != null)
            {

                var stream = audioRecorderService.GetAudioFileStream();

                fileName = Path.Combine(FileSystem.CacheDirectory, DateTime.Now.ToString("ddMMyyyymmss") + "_VoiceNote.wav");

                using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }

                AudioPath = fileName;

            }
        }

        public void guardarDatos()
        {
            if (!videoGrabado || !audioGrabado)
            {
                DisplayAlert("Advertencia", "Video O Audio no Grabados", "OK");
                return;
            }

            WebClient cliente = new WebClient();

            var parametros = new System.Collections.Specialized.NameValueCollection();
            parametros.Add("Descripcion", txtdescripcion.Text);
            parametros.Add("Latitud", txtlatitud.Text);
            parametros.Add("Longitud", txtlongitud.Text);
            parametros.Add("VideoDigital", PhotoPath);
            parametros.Add("AudioFile", AudioPath);

            cliente.UploadValues("http://192.168.1.37/Examen2PMovil2/InsertarSitio.php", "POST", parametros);
            LimpiarCampo();
        }

        private void btnlista_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Lista());
        }

        private void BtnGuardar_Clicked(object sender, EventArgs e)
        {
            Validaciones();
        }

        //metodo para validar los campos
        public void Validaciones()
        {
            if (String.IsNullOrWhiteSpace(txtlatitud.Text) || String.IsNullOrWhiteSpace(txtlongitud.Text) || String.IsNullOrWhiteSpace(txtdescripcion.Text))
            {
                this.DisplayAlert("Advertencia", "Debe de llenar los campos vacios", "OK");
            }
            else
            {
                guardarDatos();
                DisplayAlert("Guardado", "Registro Almacenado Exitosamente", "OK");
            }
        }

        //Metodo para limpiar los campos
        public void LimpiarCampo()
        {
            this.txtdescripcion.Text = String.Empty;
        }
    }
}
