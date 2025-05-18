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

    private bool _primeraVez = true;
    private CancellationTokenSource _cts;

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Iniciar el token de cancelación para refrescos automáticos
        _cts = new CancellationTokenSource();

        // Obtener el ID del usuario actual
        if (int.TryParse(COMMON.Params.UsuarioConectado, out int idUsuario))
        {
            _idUsuarioActual = idUsuario;

            // Cargar mensajes inmediatamente
            await CargarMensajesRecientes();

            // Si es la primera vez que aparece la página, iniciar refrescos automáticos
            if (_primeraVez)
            {
                _primeraVez = false;
                _ = IniciarRefrescoAutomatico(_cts.Token);
            }
        }
        else
        {
            await DisplayAlert("Error", "No hay usuario conectado", "Aceptar");
            await Shell.Current.GoToAsync("//MainPage");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Cancelar refrescos automáticos al salir de la página
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async Task IniciarRefrescoAutomatico(CancellationToken token)
    {
        try
        {
            // Refrescar cada 30 segundos
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(30000, token); // 30 segundos

                if (!token.IsCancellationRequested)
                {
                    await CargarMensajesRecientes();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Normal al cancelar, no hacer nada
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en refresco automático: {ex.Message}");
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
            try
            {
                // Debug
                Console.WriteLine($"Navegando a chat con: {chatSeleccionado.IdUsuario} - {chatSeleccionado.NombreUsuario}");

                // Crear y configurar página con valores explícitos
                var chatPage = new ChatPage(_servicioAPI, _servicioSignalR)
                {
                    UsuarioDestinatarioId = chatSeleccionado.IdUsuario,
                    NombreUsuarioDestinatario = chatSeleccionado.NombreUsuario
                };

                await Navigation.PushAsync(chatPage);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No se pudo abrir el chat", "Aceptar");
                Console.WriteLine($"Error navegación: {ex.Message}");
            }
            finally
            {
                listaChats.SelectedItem = null;
            }
        }
    }

    private async void BtnCerrarSesion_Clicked(object sender, EventArgs e)
    {
        await _servicioSignalR.Desconectar();
        COMMON.Params.UsuarioConectado = string.Empty;

        // Reiniciar completamente a un nuevo AppShell
        Application.Current.MainPage = new AppShell();
    }

    // Clase para representar un chat reciente
    public class ChatReciente
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string UltimoMensaje { get; set; }
        public DateTime FechaUltimoMensaje { get; set; }
    }

    private async void BtnPerfil_Clicked(object sender, EventArgs e)
    {
        try
        {
            var perfilPage = new Catalogos.UsuarioCatalogo(_servicioAPI, _servicioSignalR);
            await Navigation.PushAsync(perfilPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo navegar al perfil: {ex.Message}", "Aceptar");
        }
    }

    private async void BtnNuevoChat_Clicked(object sender, EventArgs e)
    {
        try
        {
            var listaUsuariosPage = new Paginas.ListaUsuariosPage(_servicioAPI);
            await Navigation.PushAsync(listaUsuariosPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo navegar a la lista de usuarios: {ex.Message}", "Aceptar");
        }
    }
}