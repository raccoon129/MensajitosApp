using COMMON.Entidades;
using Mensajitos.Servicios;

namespace Mensajitos
{
    public partial class MainPage : ContentPage
    {
        private readonly ServicioAPI _servicioAPI;
        private readonly ServicioSignalR _servicioSignalR;
        int count = 0;

        public MainPage(ServicioAPI servicioAPI, ServicioSignalR servicioSignalR)
        {
            InitializeComponent();
            _servicioAPI = servicioAPI;
            _servicioSignalR = servicioSignalR;
        }

        private async void BtnIniciarSesion_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtContrasena.Text))
            {
                await DisplayAlert("Error", "Debes completar todos los campos", "Aceptar");
                return;
            }

            // Mostrar indicador de carga
            btnIniciarSesion.IsEnabled = false;
            btnIniciarSesion.Text = "Iniciando sesión...";

            try
            {
                // Intentar iniciar sesión
                var usuario = await _servicioAPI.IniciarSesion(txtUsuario.Text, txtContrasena.Text);

                if (usuario != null)
                {
                    // Guardar usuario conectado
                    COMMON.Params.UsuarioConectado = usuario.id_usuario.ToString();

                    // Conectar con SignalR
                    await _servicioSignalR.IniciarConexion(usuario.id_usuario);

                    // Navegar a la página de mensajes recientes
                    await Shell.Current.GoToAsync(nameof(Paginas.MensajesRecientesPage));
                }
                else
                {
                    await DisplayAlert("Error", "Usuario o contraseña incorrectos", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo iniciar sesión: {ex.Message}", "Aceptar");
            }
            finally
            {
                // Restaurar botón
                btnIniciarSesion.IsEnabled = true;
                btnIniciarSesion.Text = "Iniciar Sesión";
            }
        }

        private async void IrARegistro_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Registro");
        }
    }
}
