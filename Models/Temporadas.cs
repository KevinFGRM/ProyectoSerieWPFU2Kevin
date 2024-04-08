using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace ProyectoSerieWPFU2Kevin.Models
{
    public class Temporadas
    {
        public ObservableCollection<Capitulos> ListaCapitulos { get; set; } = new ObservableCollection<Capitulos>();
        public string Titulo { get; set; } = null!;
        public int NumTemporada { get; set; }

    }
}
