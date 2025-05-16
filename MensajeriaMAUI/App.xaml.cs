using COMMON.Entidades;
using MensajeriaMAUI.Servicios;
using System.Text.Json;

namespace MensajeriaMAUI
{
    public partial class App : Application
    {
        // Usuario actual en sesión
        public static Usuario UsuarioActual { get; set; }
        
        // Servicio de chat con SignalR
        public static ChatService ChatService { get; private set; }
        
        // Clave para almacenamiento local
        private const string CLAVE_USUARIO = "UsuarioActual";

        public App()
        {
            InitializeComponent();
            
            // Inicializar el servicio de chat
            ChatService = new ChatService();
            
            // Verificar si hay un usuario con sesión iniciada
            CargarUsuarioGuardado();
            
            // Decidir qué página mostrar
            if (UsuarioActual != null)
            {
                // Si hay sesión activa, conectar al chat y mostrar AppShell
                MainThread.BeginInvokeOnMainThread(async () => {
                    await ChatService.ConectarAsync(UsuarioActual);
                    MainPage = new AppShell();
                });
            }
            else
            {
                // Si no hay sesión, mostrar la página de login
                MainPage = new MainPage();
            }
        }

        // Cargar el usuario guardado desde el almacenamiento local
        private void CargarUsuarioGuardado()
        {
            try
            {
                string usuarioJson = Preferences.Get(CLAVE_USUARIO, null);
                if (!string.IsNullOrEmpty(usuarioJson))
                {
                    UsuarioActual = JsonSerializer.Deserialize<Usuario>(usuarioJson);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar usuario: {ex.Message}");
            }
        }

        // Guardar el usuario actual en almacenamiento local
        public static void GuardarUsuarioActual()
        {
            if (UsuarioActual != null)
            {
                string usuarioJson = JsonSerializer.Serialize(UsuarioActual);
                Preferences.Set(CLAVE_USUARIO, usuarioJson);
            }
        }

        // Cerrar sesión del usuario actual
        public static async Task CerrarSesion()
        {
            if (UsuarioActual != null)
            {
                // Desconectar del chat
                await ChatService.DesconectarAsync();
                
                // Limpiar datos del usuario
                UsuarioActual = null;
                Preferences.Remove(CLAVE_USUARIO);
                
                // Volver a la página de login
                Application.Current.MainPage = new MainPage();
            }
        }
    }
}
