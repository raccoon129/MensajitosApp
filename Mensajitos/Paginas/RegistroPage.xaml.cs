using COMMON.Entidades;
using Mensajitos.Servicios;

namespace Mensajitos.Paginas;

public partial class RegistroPage : ContentPage
{
    private readonly ServicioAPI _servicioAPI;

    public RegistroPage()
    {
        InitializeComponent();
        _servicioAPI = new ServicioAPI();
    }

    private async void OnRegistrarClicked(object sender, EventArgs e)
    {
        // Validar campos
        if (string.IsNullOrWhiteSpace(EntryUsuario.Text))
        {
            LabelEstado.Text = "Por favor, ingresa un nombre de usuario";
            return;
        }

        if (string.IsNullOrWhiteSpace(EntryContrasena.Text))
        {
            LabelEstado.Text = "Por favor, ingresa una contraseña";
            return;
        }

        if (EntryContrasena.Text != EntryConfirmarContrasena.Text)
        {
            LabelEstado.Text = "Las contraseñas no coinciden";
            return;
        }

        // Mostrar indicador de actividad
        LabelEstado.TextColor = Colors.Blue;
        LabelEstado.Text = "Registrando usuario...";

        // Crear el objeto usuario
        var nuevoUsuario = new Usuario
        {
            nombre_usuario = EntryUsuario.Text,
            contrasena_hash = EntryContrasena.Text,
            fecha_ejec = DateTime.Now
        };

        try
        {
            // Llamar al método actualizado para registrar usuario
            var (usuarioRegistrado, mensaje) = await _servicioAPI.RegistrarUsuario(nuevoUsuario);

            if (usuarioRegistrado != null)
            {
                LabelEstado.TextColor = Colors.Green;
                LabelEstado.Text = "¡Usuario registrado con éxito!";

                // Esperar 2 segundos y volver a la página de login
                await Task.Delay(2000);
                await Shell.Current.GoToAsync("//Login");
            }
            else
            {
                LabelEstado.TextColor = Colors.Red;
                LabelEstado.Text = mensaje;
            }
        }
        catch (Exception ex)
        {
            LabelEstado.TextColor = Colors.Red;
            LabelEstado.Text = $"Error: {ex.Message}";
        }
    }
}