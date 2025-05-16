using MensajeriaMAUI.Servicios;
using MensajeriaMAUI.Modelos;
using System.Collections.ObjectModel;
using COMMON.Entidades;

namespace MensajeriaMAUI.Paginas
{
    public partial class ListaUsuariosPage : ContentPage
    {
        private readonly ApiService _apiService;
        private ObservableCollection<UsuarioModelo> _usuarios;
        private bool _isRefreshing;
        private bool _modoNuevaConversacion;

        public ObservableCollection<UsuarioModelo> Usuarios
        {
            get => _usuarios;
            set
            {
                _usuarios = value;
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

        public ListaUsuariosPage(bool modoNuevaConversacion = false)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _usuarios = new ObservableCollection<UsuarioModelo>();
            _modoNuevaConversacion = modoNuevaConversacion;
            RefreshCommand = new Command(async () => await CargarUsuarios());
            
            BindingContext = this;
        }

        public ListaUsuariosPage() : this(false)
        {
            // Este constructor vacío redirige al constructor con parámetros
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarUsuarios();
            
            // Suscribirse a eventos de conexión/desconexión de usuarios
            App.ChatService.UsuarioConectado += OnUsuarioConectado;
            App.ChatService.UsuarioDesconectado += OnUsuarioDesconectado;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            // Desuscribirse para evitar fugas de memoria
            App.ChatService.UsuarioConectado -= OnUsuarioConectado;
            App.ChatService.UsuarioDesconectado -= OnUsuarioDesconectado;
        }

        private async Task CargarUsuarios()
        {
            try
            {
                IsRefreshing = true;

                var usuarios = await _apiService.ObtenerUsuariosAsync();
                
                _usuarios.Clear();
                foreach (var usuario in usuarios)
                {
                    // No mostrar el usuario actual en la lista
                    if (App.UsuarioActual != null && usuario.id_usuario == App.UsuarioActual.id_usuario)
                        continue;
                        
                    _usuarios.Add(new UsuarioModelo
                    {
                        Id = usuario.id_usuario,
                        NombreUsuario = usuario.nombre_usuario,
                        IsOnLine = false // Por defecto, asumimos que están offline
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los usuarios: {ex.Message}", "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private void OnUsuarioConectado(int idUsuario)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var usuario = _usuarios.FirstOrDefault(u => u.Id == idUsuario);
                if (usuario != null)
                {
                    usuario.IsOnLine = true;
                }
            });
        }

        private void OnUsuarioDesconectado(int idUsuario)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var usuario = _usuarios.FirstOrDefault(u => u.Id == idUsuario);
                if (usuario != null)
                {
                    usuario.IsOnLine = false;
                }
            });
        }

        private async void OnUsuarioSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is UsuarioModelo usuario)
            {
                if (_modoNuevaConversacion)
                {
                    // Iniciar una nueva conversación
                    // await Navigation.PushAsync(new ChatPage(usuario.Id));
                }
                else
                {
                    // Ver perfil del usuario o mostrar opciones
                    var accion = await DisplayActionSheet(
                        $"¿Qué deseas hacer con {usuario.NombreUsuario}?",
                        "Cancelar", null,
                        "Enviar mensaje");

                    if (accion == "Enviar mensaje")
                    {
                        // await Navigation.PushAsync(new ChatPage(usuario.Id));
                    }
                }
                
                // Reset selection
                ((CollectionView)sender).SelectedItem = null;
            }
        }
    }
}