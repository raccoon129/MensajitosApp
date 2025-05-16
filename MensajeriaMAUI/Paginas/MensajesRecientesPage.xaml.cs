namespace MensajeriaMAUI.Paginas;
using MensajeriaMAUI.Servicios;
using MensajeriaMAUI.Modelos;
using COMMON.Entidades;
using System.Collections.ObjectModel;

public partial class MensajesRecientesPage : ContentPage
{
    private readonly ApiService _apiService;
    private ObservableCollection<ConversacionModelo> _conversaciones;
    private bool _isRefreshing;

    public ObservableCollection<ConversacionModelo> Conversaciones
    {
        get => _conversaciones;
        set
        {
            _conversaciones = value;
            OnPropertyChanged();
        }
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            _isRefreshing = value;
            OnPropertyChanged();
        }
    }

    public Command RefreshCommand { get; }

    public MensajesRecientesPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        Conversaciones = new ObservableCollection<ConversacionModelo>();
        RefreshCommand = new Command(async () => await CargarConversaciones());

        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (App.UsuarioActual != null)
        {
            await CargarConversaciones();
        }

        // Suscribirse a eventos de mensajes nuevos
        App.ChatService.MensajeRecibido += OnMensajeRecibido;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Desuscribirse para evitar fugas de memoria
        App.ChatService.MensajeRecibido -= OnMensajeRecibido;
    }

    private async Task CargarConversaciones()
    {
        try
        {
            IsRefreshing = true;

            // Obtener todos los mensajes del usuario actual
            var mensajes = await _apiService.ObtenerMensajesAsync(App.UsuarioActual.id_usuario);
            var usuarios = await _apiService.ObtenerUsuariosAsync();

            // Agrupar mensajes por conversación
            var conversaciones = new Dictionary<int, ConversacionModelo>();

            foreach (var mensaje in mensajes.OrderByDescending(m => m.fecha_ejec))
            {
                int otroUsuarioId;

                // Determinar si el usuario actual es emisor o receptor
                if (mensaje.emisor_id == App.UsuarioActual.id_usuario)
                {
                    otroUsuarioId = mensaje.receptor_id;
                }
                else
                {
                    otroUsuarioId = mensaje.emisor_id;
                }

                // Buscar información del otro usuario
                var otroUsuario = usuarios.FirstOrDefault(u => u.id_usuario == otroUsuarioId);
                if (otroUsuario == null) continue;

                // Si no existe la conversación, crear una nueva
                if (!conversaciones.ContainsKey(otroUsuarioId))
                {
                    conversaciones[otroUsuarioId] = new ConversacionModelo
                    {
                        UsuarioId = otroUsuarioId,
                        NombreUsuario = otroUsuario.nombre_usuario,
                        UltimoMensaje = mensaje.contenido,
                        TiempoUltimoMensaje = FormatearTiempo(mensaje.fecha_ejec),
                        MensajesNoLeidos = 0,
                        TieneMensajesNoLeidos = false
                    };
                }
            }

            Conversaciones.Clear();
            foreach (var conversacion in conversaciones.Values)
            {
                Conversaciones.Add(conversacion);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar las conversaciones: {ex.Message}", "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private void OnMensajeRecibido(Mensaje mensaje)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            // Actualizar la lista de conversaciones cuando llega un mensaje nuevo
            await CargarConversaciones();
        });
    }

    private async void OnConversacionSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ConversacionModelo conversacion)
        {
            // Navegar a la página de chat con el usuario seleccionado
            // await Navigation.PushAsync(new ChatPage(conversacion.UsuarioId));

            // Reset selection
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private async void OnNuevoMensajeClicked(object sender, EventArgs e)
    {
        // Navegar a la página de lista de usuarios para iniciar una nueva conversación
        // await Navigation.PushAsync(new ListaUsuariosPage(true));
    }

    private string FormatearTiempo(DateTime tiempo)
    {
        if (tiempo.Date == DateTime.Today)
        {
            return tiempo.ToString("HH:mm");
        }
        else if (tiempo.Date == DateTime.Today.AddDays(-1))
        {
            return "Ayer";
        }
        else if (tiempo.Date > DateTime.Today.AddDays(-7))
        {
            return tiempo.ToString("dddd");
        }
        else
        {
            return tiempo.ToString("dd/MM/yyyy");
        }
    }
}