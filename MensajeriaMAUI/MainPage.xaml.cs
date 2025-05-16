using MensajeriaMAUI.Servicios;
using COMMON.Entidades;
using Microsoft.AspNetCore.SignalR.Client;

namespace MensajeriaMAUI
{
    public partial class MainPage : ContentPage
    {
        private readonly ApiService _apiService;

        public MainPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            
            // Ocultar el indicador de carga al inicio
            IndicadorCarga.IsRunning = false;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(UsernameEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
            {
                await DisplayAlert("Error", "Por favor ingresa usuario y contraseña", "OK");
                return;
            }

            try
            {
                // Mostrar indicador de carga
                IndicadorCarga.IsRunning = true;
                BotonLogin.IsEnabled = false;
                
                // Intentar login
                var usuario = await _apiService.LoginAsync(UsernameEntry.Text, PasswordEntry.Text);

                if (usuario != null)
                {
                    // Guardar el usuario actual
                    App.UsuarioActual = usuario;
                    App.GuardarUsuarioActual();

                    // Iniciar conexión con SignalR
                    try
                    {
                        await App.ChatService.ConectarAsync(usuario);
                    }
                    catch (Exception signalREx)
                    {
                        Console.WriteLine($"No se pudo conectar a SignalR: {signalREx.Message}");
                        // Continuar a pesar del error con SignalR
                    }

                    // Ir a la página principal (Shell)
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    await DisplayAlert("Error", "Usuario o contraseña incorrectos", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al iniciar sesión: {ex.Message}", "OK");
                Console.WriteLine($"Excepción en login: {ex}");
            }
            finally
            {
                // Ocultar indicador de carga
                IndicadorCarga.IsRunning = false;
                BotonLogin.IsEnabled = true;
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Registro", "Funcionalidad no implementada aún", "OK");
        }
    }
}
