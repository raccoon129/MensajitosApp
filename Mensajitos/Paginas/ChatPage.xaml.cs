using COMMON.Entidades;
using Mensajitos.Servicios;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mensajitos.Paginas
{
    [QueryProperty(nameof(UsuarioDestinatarioId), "usuarioId")]
    [QueryProperty(nameof(NombreUsuarioDestinatario), "nombreUsuario")]
    public partial class ChatPage : ContentPage, INotifyPropertyChanged
    {
        private readonly ServicioAPI _servicioAPI;
        private readonly ServicioSignalR _servicioSignalR;

        // Propiedades para el enlace de datos
        private string _tituloPagina;
        private int _usuarioDestinatarioId;
        private string _nombreUsuarioDestinatario;
        private int _usuarioActualId;

        // Colección observable para los mensajes
        public ObservableCollection<MensajeUI> Mensajes { get; set; } = new ObservableCollection<MensajeUI>();

        public string TituloPagina
        {
            get => _tituloPagina;
            set
            {
                if (_tituloPagina != value)
                {
                    _tituloPagina = value;
                    OnPropertyChanged();
                }
            }
        }

        public int UsuarioDestinatarioId
        {
            get => _usuarioDestinatarioId;
            set
            {
                if (_usuarioDestinatarioId != value)
                {
                    _usuarioDestinatarioId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string NombreUsuarioDestinatario
        {
            get => _nombreUsuarioDestinatario;
            set
            {
                if (_nombreUsuarioDestinatario != value)
                {
                    _nombreUsuarioDestinatario = value;
                    TituloPagina = value; // Actualizar título de la página
                    OnPropertyChanged();
                }
            }
        }

        public ChatPage(ServicioAPI servicioAPI, ServicioSignalR servicioSignalR)
        {
            InitializeComponent();
            _servicioAPI = servicioAPI;
            _servicioSignalR = servicioSignalR;

            // Configurar contexto de datos para el binding
            BindingContext = this;

            // Suscribirse a eventos de SignalR
            _servicioSignalR.MensajeRecibido += OnMensajeRecibido;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Obtener ID del usuario actual
            if (!int.TryParse(COMMON.Params.UsuarioConectado, out _usuarioActualId))
            {
                await DisplayAlert("Error", "No hay usuario conectado", "Aceptar");
                await Shell.Current.GoToAsync("//MainPage");
                return;
            }

            // Cargar mensajes históricos
            await CargarMensajesHistoricos();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Desuscribirse de los eventos al salir de la página
            _servicioSignalR.MensajeRecibido -= OnMensajeRecibido;
        }

        private async Task CargarMensajesHistoricos()
        {
            try
            {
                // Obtener todos los mensajes del usuario actual
                var todosMensajes = await _servicioAPI.ObtenerMensajes(_usuarioActualId);

                // Filtrar solo los mensajes entre el usuario actual y el destinatario
                var mensajesFiltrados = todosMensajes.Where(m =>
                    (m.emisor_id == _usuarioActualId && m.receptor_id == UsuarioDestinatarioId) ||
                    (m.emisor_id == UsuarioDestinatarioId && m.receptor_id == _usuarioActualId))
                    .OrderBy(m => m.fecha_ejec)
                    .ToList();

                // Limpiar la colección actual
                Mensajes.Clear();

                // Añadir los mensajes a la colección
                foreach (var mensaje in mensajesFiltrados)
                {
                    var esPropio = mensaje.emisor_id == _usuarioActualId;
                    Mensajes.Add(new MensajeUI
                    {
                        Id = mensaje.id_mensaje,
                        Contenido = mensaje.contenido,
                        FechaHora = mensaje.fecha_ejec,
                        EsPropio = esPropio,
                        Alineacion = esPropio ? LayoutOptions.End : LayoutOptions.Start,
                        ColorFondo = esPropio ? Colors.DodgerBlue : Colors.Gray
                    });
                }

                // Desplazarse al último mensaje
                if (Mensajes.Count > 0)
                {
                    await Task.Delay(100); // Pequeña espera para asegurar que la UI se actualice
                    listaMensajes.ScrollTo(Mensajes.Last(), animate: false);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los mensajes: {ex.Message}", "Aceptar");
            }
        }

        private async void BtnEnviar_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMensaje.Text))
                return;

            try
            {
                // Crear nuevo mensaje
                var nuevoMensaje = new Mensaje
                {
                    emisor_id = _usuarioActualId,
                    receptor_id = UsuarioDestinatarioId,
                    contenido = txtMensaje.Text,
                    fecha_ejec = DateTime.Now
                };

                // Enviar mensaje a través de SignalR
                await _servicioSignalR.EnviarMensaje(nuevoMensaje);

                // Guardar mensaje en la base de datos a través de la API
                await _servicioAPI.GuardarMensaje(nuevoMensaje);

                // Añadir mensaje a la UI
                Mensajes.Add(new MensajeUI
                {
                    Id = nuevoMensaje.id_mensaje,
                    Contenido = nuevoMensaje.contenido,
                    FechaHora = nuevoMensaje.fecha_ejec,
                    EsPropio = true,
                    Alineacion = LayoutOptions.End,
                    ColorFondo = Colors.DodgerBlue
                });

                // Limpiar campo de texto
                txtMensaje.Text = string.Empty;

                // Desplazarse al último mensaje
                await Task.Delay(100);
                listaMensajes.ScrollTo(Mensajes.Last(), animate: true);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo enviar el mensaje: {ex.Message}", "Aceptar");
            }
        }

        private void OnMensajeRecibido(Mensaje mensaje)
        {
            // Verificar que el mensaje es parte de esta conversación
            if ((mensaje.emisor_id == _usuarioActualId && mensaje.receptor_id == UsuarioDestinatarioId) ||
                (mensaje.emisor_id == UsuarioDestinatarioId && mensaje.receptor_id == _usuarioActualId))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var esPropio = mensaje.emisor_id == _usuarioActualId;

                    // Evitar duplicados (si el mensaje ya está en la lista)
                    if (!Mensajes.Any(m => m.Id == mensaje.id_mensaje))
                    {
                        Mensajes.Add(new MensajeUI
                        {
                            Id = mensaje.id_mensaje,
                            Contenido = mensaje.contenido,
                            FechaHora = mensaje.fecha_ejec,
                            EsPropio = esPropio,
                            Alineacion = esPropio ? LayoutOptions.End : LayoutOptions.Start,
                            ColorFondo = esPropio ? Colors.DodgerBlue : Colors.Gray
                        });

                        // Desplazarse al último mensaje
                        listaMensajes.ScrollTo(Mensajes.Last(), animate: true);
                    }
                });
            }
        }

        // Implementación de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Clase para representar un mensaje en la UI
    public class MensajeUI
    {
        public int Id { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaHora { get; set; }
        public bool EsPropio { get; set; }
        public LayoutOptions Alineacion { get; set; }
        public Color ColorFondo { get; set; }
    }
}