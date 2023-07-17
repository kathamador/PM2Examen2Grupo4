using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PM2Examen2Grupo4
{
    public class sitios
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string VideoDigital { get; set; } 
        public string AudioFile { get; set; } 

    }
}
