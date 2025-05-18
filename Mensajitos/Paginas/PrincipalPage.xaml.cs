using Mensajitos.Catalogos;
using Mensajitos.Servicios;

namespace Mensajitos.Paginas
{
    public partial class PrincipalPage : ContentPage
    {
        private readonly ServicioAPI _servicioAPI;
        private readonly ServicioSignalR _servicioSignalR;

        private MensajesRecientesPage _mensajesPage;
        private ListaUsuariosPage _contactosPage;
        private UsuarioCatalogo _perfilPage;

        // Botón actualmente seleccionado
        private Button _botonActual;

        public PrincipalPage(ServicioAPI servicioAPI, ServicioSignalR servicioSignalR)
        {
            InitializeComponent();
            _servicioAPI = servicioAPI;
            _servicioSignalR = servicioSignalR;

            // Inicializar las páginas
            _mensajesPage = new MensajesRecientesPage(_servicioAPI, _servicioSignalR);
            _contactosPage = new ListaUsuariosPage(_servicioAPI);
            _perfilPage = new UsuarioCatalogo(_servicioAPI, _servicioSignalR);

            // Establecer la página inicial
            contentArea.Content = _mensajesPage.Content;
            _botonActual = btnMensajes;
        }

        private void ActivarBoton(Button boton)
        {
            // Restablecer todos los botones
            btnMensajes.TextColor = Color.FromArgb("#808080");
            btnContactos.TextColor = Color.FromArgb("#808080");
            btnPerfil.TextColor = Color.FromArgb("#808080");

            // Activar el botón seleccionado
            boton.TextColor = Color.FromArgb("#512BD4");
            _botonActual = boton;
        }

        private void BtnMensajes_Clicked(object sender, EventArgs e)
        {
            contentArea.Content = _mensajesPage.Content;
            ActivarBoton(btnMensajes);
            Title = "Mensajes";
        }

        private void BtnContactos_Clicked(object sender, EventArgs e)
        {
            contentArea.Content = _contactosPage.Content;
            ActivarBoton(btnContactos);
            Title = "Contactos";
        }

        private void BtnPerfil_Clicked(object sender, EventArgs e)
        {
            contentArea.Content = _perfilPage.Content;
            ActivarBoton(btnPerfil);
            Title = "Mi Perfil";
        }
    }
}