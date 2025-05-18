using COMMON.Entidades;
using Mensajitos.Servicios;

namespace Mensajitos.Catalogos
{
    public partial class UsuarioCatalogo : ContentPage
    {
        private readonly ServicioAPI _servicioAPI;
        private readonly ServicioSignalR _servicioSignalR;
        private Usuario _usuarioActual;

        public UsuarioCatalogo(ServicioAPI servicioAPI, ServicioSignalR servicioSignalR)
        {
            InitializeComponent();
            _servicioAPI = servicioAPI;
            _servicioSignalR = servicioSignalR;

            // Deshabilitar campos de edici�n por ahora
            txtNombreUsuario.IsReadOnly = true;
            txtContrase�aActual.IsEnabled = false;
            txtNuevaContrase�a.IsEnabled = false;
            txtConfirmarContrase�a.IsEnabled = false;
            btnGuardarCambios.IsEnabled = false;
            btnGuardarCambios.Opacity = 0.5;

            // Agregar mensaje informativo
            btnGuardarCambios.Text = "Edici�n no disponible";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarDatosUsuario();
        }

        private async Task CargarDatosUsuario()
        {
            try
            {
                // Obtener el ID del usuario conectado
                if (string.IsNullOrEmpty(COMMON.Params.UsuarioConectado))
                {
                    await DisplayAlert("Error", "No hay usuario conectado", "Aceptar");
                    return;
                }

                int idUsuario = int.Parse(COMMON.Params.UsuarioConectado);

                // Obtener todos los usuarios para encontrar el usuario actual
                var usuarios = await _servicioAPI.ObtenerUsuarios();
                _usuarioActual = usuarios.FirstOrDefault(u => u.id_usuario == idUsuario);

                if (_usuarioActual != null)
                {
                    // Mostrar datos en los controles
                    lblIdUsuario.Text = _usuarioActual.id_usuario.ToString();
                    txtNombreUsuario.Text = _usuarioActual.nombre_usuario;

                    // No mostrar contrase�a por seguridad
                    txtContrase�aActual.Text = "��������";
                    txtNuevaContrase�a.Text = string.Empty;
                    txtConfirmarContrase�a.Text = string.Empty;
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo obtener la informaci�n del usuario", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar datos: {ex.Message}", "Aceptar");
            }
        }

        // M�todo que se ejecutar�a cuando se habilite la edici�n
        private async void BtnGuardarCambios_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Informaci�n", "La edici�n de perfil estar� disponible pr�ximamente", "Entendido");
        }

        private async void BtnCerrarSesion_Clicked(object sender, EventArgs e)
        {
            bool confirmacion = await DisplayAlert("Cerrar Sesi�n",
                "�Est�s seguro que deseas cerrar sesi�n?",
                "S�", "No");

            if (confirmacion)
            {
                try
                {
                    // Desconectar SignalR
                    await _servicioSignalR.Desconectar();

                    // Limpiar datos de usuario
                    COMMON.Params.UsuarioConectado = string.Empty;

                    // Navegar a login
                    Application.Current.MainPage = new AppShell();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Error al cerrar sesi�n: {ex.Message}", "Aceptar");
                }
            }
        }
    }
}