using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoSerieWPFU2Kevin.Models
{
    public class Capitulos
    {
        public string Titulo { get; set; } = null!;
        public string URL { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public int NumeroCapitulo {  get; set; }
        public DateTime Fecha { get; set; }
    }
}
