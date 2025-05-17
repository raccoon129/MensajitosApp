using COMMON.Entidades;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Mensajitos.Servicios
{
    public class ServicioAPI
    {
        private readonly HttpClient _cliente;
        
        public ServicioAPI()
        {
            _cliente = new HttpClient
            {
                BaseAddress = new Uri(COMMON.Params.UrlAPI)
            };
        }
        
        public async Task<Usuario?> IniciarSesion(string nombreUsuario, string contrasena)
        {
            try
            {
                var modelo = new
                {
                    NombreUsuario = nombreUsuario,
                    Contrasena = contrasena
                };
                
                var respuesta = await _cliente.PostAsJsonAsync("api/Usuario/login", modelo);
                
                if (respuesta.IsSuccessStatusCode)
                {
                    return await respuesta.Content.ReadFromJsonAsync<Usuario>();
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al iniciar sesión: {ex.Message}");
                return null;
            }
        }
        
        public async Task<List<Mensaje>> ObtenerMensajes(int idUsuario)
        {
            try
            {
                var respuesta = await _cliente.GetAsync($"api/Mensaje?id={idUsuario}");
                
                if (respuesta.IsSuccessStatusCode)
                {
                    return await respuesta.Content.ReadFromJsonAsync<List<Mensaje>>() ?? new List<Mensaje>();
                }
                
                return new List<Mensaje>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener mensajes: {ex.Message}");
                return new List<Mensaje>();
            }
        }
        
        public async Task<List<Usuario>> ObtenerUsuarios()
        {
            try
            {
                var respuesta = await _cliente.GetAsync("api/Usuario");
                
                if (respuesta.IsSuccessStatusCode)
                {
                    return await respuesta.Content.ReadFromJsonAsync<List<Usuario>>() ?? new List<Usuario>();
                }
                
                return new List<Usuario>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                return new List<Usuario>();
            }
        }

        // Añadir este método a la clase ServicioAPI
        public async Task<Mensaje> GuardarMensaje(Mensaje mensaje)
        {
            try
            {
                var respuesta = await _cliente.PostAsJsonAsync("api/Mensaje", mensaje);

                if (respuesta.IsSuccessStatusCode)
                {
                    return await respuesta.Content.ReadFromJsonAsync<Mensaje>();
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar mensaje: {ex.Message}");
                return null;
            }
        }
    }
}