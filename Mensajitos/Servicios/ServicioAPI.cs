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
        public async Task<Usuario> ActualizarUsuario(Usuario usuario)
        {
            try
            {
                var json = JsonContent.Create(usuario);
                var respuesta = await _cliente.PutAsync($"{COMMON.Params.UrlAPI}api/Usuario", json);

                if (respuesta.IsSuccessStatusCode)
                {
                    var contenido = await respuesta.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Usuario>(contenido, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Aaaaa para crear un usuario
        public async Task<(Usuario usuario, string mensaje)> RegistrarUsuario(Usuario usuario)
        {
            try
            {
                // Asegurarse de que la fecha de ejecución esté configurada
                if (usuario.fecha_ejec == default)
                {
                    usuario.fecha_ejec = DateTime.Now;
                }

                // Validar campos obligatorios antes de enviar
                if (string.IsNullOrWhiteSpace(usuario.nombre_usuario))
                    return (null, "El nombre de usuario es obligatorio");

                if (string.IsNullOrWhiteSpace(usuario.contrasena_hash))
                    return (null, "La contraseña es obligatoria");

                // Crear una copia del objeto sin ninguna propiedad que pueda ser problemática
                var usuarioDTO = new
                {
                    nombre_usuario = usuario.nombre_usuario,
                    contrasena_hash = usuario.contrasena_hash,
                    fecha_ejec = usuario.fecha_ejec
                };

                var respuesta = await _cliente.PostAsJsonAsync("api/Usuario", usuarioDTO);

                string contenidoRespuesta = await respuesta.Content.ReadAsStringAsync();

                if (respuesta.IsSuccessStatusCode)
                {
                    try
                    {
                        var usuarioRegistrado = JsonSerializer.Deserialize<Usuario>(contenidoRespuesta,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        return (usuarioRegistrado, "Registro exitoso");
                    }
                    catch
                    {
                        return (null, "Error al procesar la respuesta del servidor");
                    }
                }

                // Mejor manejo del mensaje de error
                return (null, $"Error del servidor: {respuesta.StatusCode} - {contenidoRespuesta}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar usuario: {ex.Message}");
                return (null, $"Error de conexión: {ex.Message}");
            }
        }
    }
}