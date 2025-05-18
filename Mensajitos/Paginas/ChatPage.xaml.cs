using COMMON.Entidades;
using Mensajitos.Servicios;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mensajitos.Paginas
{
    public partial class ChatPage : ContentPage, INotifyPropertyChanged
    {
        private readonly ServicioAPI _servicioAPI;
        private readonly ServicioSignalR _servicioSignalR;
        private int _usuarioActualId;
        private int _usuarioDestinatarioId;
        private string _nombreUsuarioDestinatario;

        // Colección para mensajes
        public ObservableCollection<MensajeUI> Mensajes { get; } = new ObservableCollection<MensajeUI>();

        // Propiedades básicas
        public int UsuarioDestinatarioId
        {
            get => _usuarioDestinatarioId;
            set
            {
                _usuarioDestinatarioId = value;
                OnPropertyChanged();
            }
        }

        public string NombreUsuarioDestinatario
        {
            get => _nombreUsuarioDestinatario;
            set
            {
                _nombreUsuarioDestinatario = value;
                Title = value;
                OnPropertyChanged();
            }
        }

        public ChatPage(ServicioAPI servicioAPI, ServicioSignalR servicioSignalR)
        {
            InitializeComponent();
            _servicioAPI = servicioAPI;
            _servicioSignalR = servicioSignalR;
            BindingContext = this;

            // Suscribirse a evento de mensajes
            _servicioSignalR.MensajeRecibido += OnMensajeRecibido;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Obtener ID del usuario conectado
            if (!int.TryParse(COMMON.Params.UsuarioConectado, out _usuarioActualId))
            {
                await Navigation.PopAsync();
                return;
            }

            // Cargar mensajes
            await CargarMensajes();
        }

        private async Task CargarMensajes()
        {
            try
            {
                // Obtener mensajes directamente
                var mensajes = await _servicioAPI.ObtenerConversacion(_usuarioActualId, UsuarioDestinatarioId);

                // Limpiar y mostrar
                Mensajes.Clear();

                foreach (var msg in mensajes)
                {
                    bool esPropio = msg.emisor_id == _usuarioActualId;
                    Mensajes.Add(new MensajeUI
                    {
                        Id = msg.id_mensaje,
                        Contenido = msg.contenido ?? "",
                        FechaHora = msg.fecha_ejec,
                        EsPropio = esPropio,
                        Alineacion = esPropio ? LayoutOptions.End : LayoutOptions.Start,
                        ColorFondo = esPropio ? Colors.DodgerBlue : Colors.Gray
                    });
                }

                // Scroll al último mensaje
                if (Mensajes.Count > 0)
                {
                    await Task.Delay(100);
                    listaMensajes.ScrollTo(Mensajes.Last(), animate: false);
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "No se pudieron cargar los mensajes", "Aceptar");
            }
        }

        private async void BtnEnviar_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMensaje.Text))
                return;

            try
            {
                btnEnviar.IsEnabled = false;

                // Crear y guardar mensaje
                var mensaje = new Mensaje
                {
                    emisor_id = _usuarioActualId,
                    receptor_id = UsuarioDestinatarioId,
                    contenido = txtMensaje.Text,
                    fecha_ejec = DateTime.Now
                };

                var guardado = await _servicioAPI.GuardarMensaje(mensaje);
                if (guardado != null)
                {
                    // Enviar por SignalR
                    await _servicioSignalR.EnviarMensaje(guardado);

                    // Añadir a la UI
                    Mensajes.Add(new MensajeUI
                    {
                        Id = guardado.id_mensaje,
                        Contenido = guardado.contenido,
                        FechaHora = guardado.fecha_ejec,
                        EsPropio = true,
                        Alineacion = LayoutOptions.End,
                        ColorFondo = Colors.DodgerBlue
                    });

                    // Limpiar y hacer scroll
                    txtMensaje.Text = string.Empty;
                    listaMensajes.ScrollTo(Mensajes.Last(), animate: true);
                }
            }
            finally
            {
                btnEnviar.IsEnabled = true;
            }
        }

        private void OnMensajeRecibido(Mensaje mensaje)
        {
            // Verificar si el mensaje pertenece a esta conversación
            if ((mensaje.emisor_id == _usuarioActualId && mensaje.receptor_id == UsuarioDestinatarioId) ||
                (mensaje.emisor_id == UsuarioDestinatarioId && mensaje.receptor_id == _usuarioActualId))
            {
                MainThread.BeginInvokeOnMainThread(() => {
                    // Evitar duplicados
                    if (!Mensajes.Any(m => m.Id == mensaje.id_mensaje))
                    {
                        // Añadir a la UI
                        Mensajes.Add(new MensajeUI
                        {
                            Id = mensaje.id_mensaje,
                            Contenido = mensaje.contenido,
                            FechaHora = mensaje.fecha_ejec,
                            EsPropio = mensaje.emisor_id == _usuarioActualId,
                            Alineacion = mensaje.emisor_id == _usuarioActualId ? LayoutOptions.End : LayoutOptions.Start,
                            ColorFondo = mensaje.emisor_id == _usuarioActualId ? Colors.DodgerBlue : Colors.Gray
                        });

                        // Hacer scroll
                        listaMensajes.ScrollTo(Mensajes.Last(), animate: true);
                    }
                });
            }
        }

        protected override void OnDisappearing()
        {
            _servicioSignalR.MensajeRecibido -= OnMensajeRecibido;
            base.OnDisappearing();
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

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