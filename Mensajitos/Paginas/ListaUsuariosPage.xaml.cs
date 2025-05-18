using COMMON.Entidades;
using Mensajitos.Servicios;
using System.Windows.Input;

namespace Mensajitos.Paginas
{
    public partial class ListaUsuariosPage : ContentPage
    {
        private readonly ServicioAPI _servicioAPI;
        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }

        public ListaUsuariosPage(ServicioAPI servicioAPI)
        {
            InitializeComponent();
            _servicioAPI = servicioAPI;

            RefreshCommand = new Command(async () => await CargarUsuarios());
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarUsuarios();
            //Title = NombreUsuarioDestino;

        }

        private async Task CargarUsuarios()
        {
            try
            {
                IsRefreshing = true;

                // Obtener usuarios desde la API
                var usuarios = await _servicioAPI.ObtenerUsuarios();

                // Filtrar para no mostrar al usuario actual
                if (!string.IsNullOrEmpty(COMMON.Params.UsuarioConectado))
                {
                    int usuarioActualId = int.Parse(COMMON.Params.UsuarioConectado);
                    usuarios = usuarios.Where(u => u.id_usuario != usuarioActualId).ToList();
                }

                // Asignar a la lista
                listaUsuarios.ItemsSource = usuarios;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los usuarios: {ex.Message}", "Aceptar");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async void ListaUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Usuario usuarioSeleccionado)
            {
                // Desactivar la selección para evitar problemas visuales
                listaUsuarios.SelectedItem = null;

                // Preguntar si desea iniciar chat
                bool iniciarChat = await DisplayAlert("Nuevo chat",
                    $"¿Deseas iniciar una conversación con {usuarioSeleccionado.nombre_usuario}?",
                    "Sí", "No");

                if (iniciarChat)
                {
                    try
                    {
                        // Verificar si el servicio SignalR está disponible
                        var servicioSignalR = Application.Current.Handler.MauiContext.Services.GetService<ServicioSignalR>();
                        if (servicioSignalR == null)
                        {
                            await DisplayAlert("Error", "No se pudo inicializar el servicio de chat", "Aceptar");
                            return;
                        }

                        // Crear una instancia de ChatPage y establecer propiedades
                        var chatPage = new ChatPage(_servicioAPI, servicioSignalR)
                        {
                            UsuarioDestinoId = usuarioSeleccionado.id_usuario,
                            NombreUsuarioDestino = usuarioSeleccionado.nombre_usuario ?? "Usuario"
                        };

                        // Navegar a la página de chat usando NavigationPage
                        await Navigation.PushAsync(chatPage);
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"No se pudo iniciar la conversación: {ex.Message}", "Aceptar");
                    }
                }
            }
        }
    }
}