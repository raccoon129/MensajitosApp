using COMMON.Entidades;
using Mensajitos.Servicios;
using System.Collections.ObjectModel;

namespace Mensajitos.Paginas;

public partial class MensajesRecientesPage : ContentPage
{
    private readonly ServicioAPI _servicioAPI;
    private readonly ServicioSignalR _servicioSignalR;
    private ObservableCollection<ChatReciente> _chatsRecientes = new ObservableCollection<ChatReciente>();
    private int _idUsuarioActual;

    public MensajesRecientesPage(ServicioAPI servicioAPI, ServicioSignalR servicioSignalR)
    {
        InitializeComponent();
        _servicioAPI = servicioAPI;
        _servicioSignalR = servicioSignalR;
        
        // Suscribirse al evento de mensaje recibido
        _servicioSignalR.MensajeRecibido += OnMensajeRecibido;
        
        listaChats.ItemsSource = _chatsRecientes;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Obtener el ID del usuario actual
        if (int.TryParse(COMMON.Params.UsuarioConectado, out int idUsuario))
        {
            _idUsuarioActual = idUsuario;
            await CargarMensajesRecientes();
        }
        else
        {
            await DisplayAlert("Error", "No hay usuario conectado", "Aceptar");
            await Shell.Current.GoToAsync("//MainPage");
        }
    }

    private async Task CargarMensajesRecientes()
    {
        try
        {
            var mensajes = await _servicioAPI.ObtenerMensajes(_idUsuarioActual);
            var usuarios = await _servicioAPI.ObtenerUsuarios();
            
            // Agrupar mensajes por usuario (conversación)
            var chatsAgrupados = mensajes
                .GroupBy(m => m.emisor_id == _idUsuarioActual ? m.receptor_id : m.emisor_id)
                .Select(g => {
                    var otroUsuarioId = g.Key;
                    var ultimoMensaje = g.OrderByDescending(m => m.fecha_ejec).First();
                    var otroUsuario = usuarios.FirstOrDefault(u => u.id_usuario == otroUsuarioId);
                    
                    return new ChatReciente
                    {
                        IdUsuario = otroUsuarioId,
                        NombreUsuario = otroUsuario?.nombre_usuario ?? "Usuario Desconocido",
                        UltimoMensaje = ultimoMensaje.contenido,
                        FechaUltimoMensaje = ultimoMensaje.fecha_ejec
                    };
                })
                .OrderByDescending(c => c.FechaUltimoMensaje)
                .ToList();
            
            _chatsRecientes.Clear();
            foreach (var chat in chatsAgrupados)
            {
                _chatsRecientes.Add(chat);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar los mensajes: {ex.Message}", "Aceptar");
        }
    }

    private void OnMensajeRecibido(Mensaje mensaje)
    {
        // Actualizar la lista de chats recientes cuando se recibe un mensaje
        MainThread.BeginInvokeOnMainThread(async () => {
            await CargarMensajesRecientes();
        });
    }

    private async void ListaChats_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ChatReciente chatSeleccionado)
        {
            // Aquí se navegaría a la pantalla de chat con el usuario seleccionado
            // Por simplicidad, solo mostramos un mensaje
            await DisplayAlert("Chat", $"Abriendo chat con {chatSeleccionado.NombreUsuario}", "Aceptar");
            listaChats.SelectedItem = null;
        }
    }

    private async void BtnNuevoChat_Clicked(object sender, EventArgs e)
    {
        // Aquí se implementaría la lógica para iniciar un nuevo chat
        await DisplayAlert("Nuevo Chat", "Próximamente: Lista de usuarios disponibles", "Aceptar");
    }

    private async void BtnCerrarSesion_Clicked(object sender, EventArgs e)
    {
        await _servicioSignalR.Desconectar();
        COMMON.Params.UsuarioConectado = string.Empty;
        await Shell.Current.GoToAsync("//MainPage");
    }
    
    // Clase para representar un chat reciente
    public class ChatReciente
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string UltimoMensaje { get; set; }
        public DateTime FechaUltimoMensaje { get; set; }
    }
}