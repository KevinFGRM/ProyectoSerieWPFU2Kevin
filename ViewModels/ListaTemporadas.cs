using ProyectoSerieWPFU2Kevin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace ProyectoSerieWPFU2Kevin.ViewModels
{
    public enum Vistas { Lista, AgregarT, EditarT, EliminarT, AgregarC, EditarC, EliminarC, Detalles }
    public class ListaTemporadas:INotifyPropertyChanged
    {
       public ObservableCollection<Temporadas> ListaDeTemporadas { get; set; } = new ObservableCollection<Temporadas>();
       private Temporadas temporada;

       public Temporadas Temporada
       {
           get { return temporada; }
           set
           {
               if (temporada != value)
               {
                   temporada = value;
                    CambiarVista(Vistas.Lista);
                   Actualizar(nameof(Temporada));
               }
           }
       }
        public Capitulos Capitulo { get; set; }
        public Vistas Vista { get; set; } = Vistas.Lista;
        public string Error { get; set; } = "";

        public ICommand AgregarEpisodioCommand { get; set; }
        public ICommand GuardarEpisodioCommand { get; set; }
        public ICommand EliminarEpisodioCommand { get; set; }
        public ICommand AgregarTemporadaCommand { get; set; }
        public ICommand GuardarTemporadaCommand { get; set; }
        public ICommand EliminarTemporadaCommand { get; set; }
        public ICommand CambiarVistaCommand { get; set; }

        int posCapituloEditar;
        int posTemporadaEditar;
        private void CambiarVista(Vistas vistas)
        {
            Vista = vistas;
            if (Vista == Vistas.AgregarC)
            {
                Capitulo = new()
                {
                    Fecha = DateTime.Now.Date
                };

                Actualizar(nameof(Vista));
            }
            if (Vista == Vistas.AgregarT)
            {
                Temporada = new();
                Actualizar(nameof(Vista));

            }
            if (Vista == Vistas.EditarC && Capitulo != null)
            {
                var clon = new Capitulos
                {
                    Descripcion = Capitulo.Descripcion,
                    Titulo = Capitulo.Titulo,
                    URL = Capitulo.URL,
                    Fecha = Capitulo.Fecha,
                    NumeroCapitulo = Capitulo.NumeroCapitulo
                };
                posCapituloEditar = Temporada.ListaCapitulos.IndexOf(Capitulo);
                Capitulo = clon;
                Actualizar(nameof(Capitulo));

            }
            if (Vista == Vistas.EditarT && Temporada != null)
            {
                var clon = new Temporadas()
                {
                    Titulo = Temporada.Titulo,
                    NumTemporada = Temporada.NumTemporada,
                    ListaCapitulos = Temporada.ListaCapitulos
                };
                posTemporadaEditar = ListaDeTemporadas.IndexOf(Temporada);
                Temporada = clon;
                Actualizar(nameof(Temporada));

            }
            Error = "";
            Actualizar(nameof(Temporada));
            Actualizar(nameof(Capitulo));
            Actualizar(nameof(Error));
            Vista = vistas;
            Actualizar(nameof(Vista));
        }

        public ListaTemporadas()
        {
            AgregarEpisodioCommand = new RelayCommand<Capitulos>(AgregarCapitulo);
            AgregarTemporadaCommand = new RelayCommand<Temporadas>(AgregarTemporada);
            GuardarEpisodioCommand = new RelayCommand<Capitulos>(EditarCapitulos);
            GuardarTemporadaCommand = new RelayCommand<Temporadas>(EditarTemporada);
            EliminarEpisodioCommand = new RelayCommand(EliminarCapitulo);
            EliminarTemporadaCommand = new RelayCommand(EliminarTemporada);

            CambiarVistaCommand = new RelayCommand<Vistas>(CambiarVista);

            Deserializar();
        }

        public void EditarTemporada(Temporadas? t)
        {
            t = Temporada;
            if (t != null)
            {
                if (string.IsNullOrWhiteSpace(t.Titulo))
                {
                    Error = "Escribe el titulo de la temporada";
                }
                if (ListaDeTemporadas[posTemporadaEditar].NumTemporada != t.NumTemporada)
                {
                    if (t.NumTemporada == 0 || ListaDeTemporadas.Any(x => x.NumTemporada == t.NumTemporada))
                    {
                        Error = "Escribe un Numero de la temporada valido";
                    }
                }
                if (Error == "")
                {
                    ListaDeTemporadas[posTemporadaEditar] = t;
                    var clon = ListaDeTemporadas.OrderBy(x => x.NumTemporada).ToList();
                    ListaDeTemporadas = new ObservableCollection<Temporadas>(clon);
                    Actualizar(nameof(ListaDeTemporadas));
                    Serializar();
                    CambiarVista(Vistas.Lista);
                }
                Actualizar(nameof(Error));
                Error = "";
            }
        }
        public void EditarCapitulos(Capitulos? c)
        {
            c = Capitulo;
            if (c != null)
            {
                if (string.IsNullOrWhiteSpace(c.Titulo))
                {
                    Error = "Escribe el titulo del capitulo";
                }
                if (string.IsNullOrWhiteSpace(c.URL))
                {
                    Error = "Escribe una URL";
                }
                else if (!c.URL.StartsWith("http") || !c.URL.EndsWith(".jpg"))
                {
                    Error = "Escriba la URL de la Imagen en formato JPG.";
                }
                if (string.IsNullOrWhiteSpace(c.Descripcion))
                {
                    Error = "Escribe la descripcion del capitulo";

                }
                if (Temporada.ListaCapitulos[posCapituloEditar].NumeroCapitulo != c.NumeroCapitulo)
                {
                    if (Temporada.ListaCapitulos.Any(x => x.NumeroCapitulo == c.NumeroCapitulo))
                    {
                        Error = "Escribe un numero de capitulo valido";
                    }
                }
                if (c.Fecha.Date > DateTime.Now.Date)
                {
                    Error = "- No se deben redactar noticias a futuro.";
                }
                if (Error == "")
                {
                    Temporada.ListaCapitulos[posCapituloEditar] = c;
                    var clon = Temporada.ListaCapitulos.OrderBy(x => x.NumeroCapitulo).ToList();
                    Temporada.ListaCapitulos = new ObservableCollection<Capitulos>(clon);

                    Serializar();
                    CambiarVista(Vistas.Lista);
                }
                Actualizar(nameof(Error));
                Error = "";
            }
        }

        public void EliminarCapitulo()
        {
            if (Capitulo != null)
            {
                if(Temporada.ListaCapitulos.Count() == 1)
                {
                    ListaDeTemporadas.Remove(Temporada);
                }
                Temporada.ListaCapitulos.Remove(Capitulo);
                Serializar();
                CambiarVista(Vistas.Lista);
            }
        }
        private void EliminarTemporada()
        {
            if(Temporada != null)
            {
                ListaDeTemporadas.Remove(Temporada);
                var clon = ListaDeTemporadas.OrderBy(x => x.NumTemporada).ToList();
                ListaDeTemporadas = new ObservableCollection<Temporadas>(clon);
                Actualizar(nameof(ListaDeTemporadas));
                Serializar();
                CambiarVista(Vistas.Lista);
            }
        }
        public void AgregarCapitulo(Capitulos? c)
        {
            c = Capitulo;
            if (c != null)
            {
                if (string.IsNullOrWhiteSpace(c.Titulo))
                {
                    Error = "Escribe el titulo del capitulo";
                }
                if (string.IsNullOrWhiteSpace(c.URL))
                {
                    Error = "Escribe una URL";
                }
                else if (!c.URL.StartsWith("http") || !c.URL.EndsWith(".jpg"))
                {
                    Error = "Escriba la URL de la Imagen en formato JPG.";
                }
                if (string.IsNullOrWhiteSpace(c.Descripcion))
                {
                    Error = "Escribe la descripcion del capitulo";
                }
                if(Temporada.ListaCapitulos.Any(x => x.NumeroCapitulo == c.NumeroCapitulo))
                {
                    Error = "Escribe un numero de capitulo valido";
                }
                if (c.Fecha.Date > DateTime.Now.Date)
                {
                    Error = "- No se deben redactar noticias a futuro.";
                }
                if (Error == "")
                {
                    Temporada.ListaCapitulos.Add(c);
                    var clon = Temporada.ListaCapitulos.OrderBy(x => x.NumeroCapitulo).ToList();
                    Temporada.ListaCapitulos = new ObservableCollection<Capitulos>(clon);
                    Serializar();
                    CambiarVista(Vistas.Lista);
                }
                Actualizar(nameof(Error));
                Error = "";
            }

        }
        public void AgregarTemporada(Temporadas? t)
        {
            t = Temporada;
            if(t != null)
            {
                if(string.IsNullOrWhiteSpace(t.Titulo))
                {
                    Error = "Escribe el titulo de la temporada";
                }
                if(t.NumTemporada == 0 || ListaDeTemporadas.Any(x => x.NumTemporada == t.NumTemporada))
                {
                    Error = "Escribe un Numero de la temporada valido";

                }
                if (Error == "")
                {
                    ListaDeTemporadas.Add(t);
                    var clon = ListaDeTemporadas.OrderBy(x => x.NumTemporada).ToList();
                    ListaDeTemporadas = new ObservableCollection<Temporadas>(clon);
                    Actualizar(nameof(ListaDeTemporadas));
                    Serializar();
                    CambiarVista(Vistas.Lista);
                    CambiarVista(Vistas.AgregarC);
                }
                Actualizar(nameof(Error));
                Error = "";

            }

        }

        private void Serializar()
        {

            File.WriteAllText("Temporadas.Json", JsonSerializer.Serialize(ListaDeTemporadas));
        }
        private void Deserializar()
        {
            if (File.Exists("Temporadas.Json"))
            {
                var datos =
                JsonSerializer.Deserialize<ObservableCollection<Temporadas>>(File.ReadAllText("Temporadas.Json"));

                if (datos != null)
                {
                    foreach (var n in datos)
                    {
                        ListaDeTemporadas.Add(n);
                    }
                }
            }
        }
        private void Actualizar(string? name)
        {
            PropertyChanged?.Invoke(this, new(name));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
