namespace Mensajitos
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar rutas para navegación
            Routing.RegisterRoute("MensajesRecientes", typeof(Paginas.MensajesRecientesPage));
            Routing.RegisterRoute("ListaUsuarios", typeof(Paginas.ListaUsuariosPage));
            Routing.RegisterRoute("UsuarioPerfil", typeof(Catalogos.UsuarioCatalogo));
            Routing.RegisterRoute("ChatPage", typeof(Paginas.ChatPage));
            Routing.RegisterRoute("Registro", typeof(Paginas.RegistroPage));
        }

        public async Task IrAMensajesRecientes()
        {
            try
            {
                // Evitamos usar rutas absolutas que causan el error
                // Creamos directamente la página y la establecemos como página principal
                var servicioAPI = Application.Current.Handler.MauiContext.Services.GetService<Servicios.ServicioAPI>();
                var servicioSignalR = Application.Current.Handler.MauiContext.Services.GetService<Servicios.ServicioSignalR>();

                // Reemplazar toda la página principal con la nueva página
                Application.Current.MainPage = new NavigationPage(new Paginas.MensajesRecientesPage(servicioAPI, servicioSignalR));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al navegar: {ex.Message}", "Aceptar");
            }
        }
    }
}