using COMMON.Entidades;
using MensajeriaWebAPI.Modelos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MensajeriaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : GenericController<Usuario>
    {
        public UsuarioController() : base(Parametros.FabricaRepository.UsuarioRepository())
        {
        }

        [HttpPost("login")]
        public ActionResult<Usuario> Login([FromBody] LoginModeloAPI model)
        {
            if (string.IsNullOrEmpty(model.NombreUsuario) || string.IsNullOrEmpty(model.Contrasena))
            {
                return BadRequest("Nombre de usuario y contraseña son requeridos");
            }

            try
            {
                // Obtener todos los usuarios con manejo seguro de null
                var usuarios = _repositorio.ObtenerTodas() ?? new List<Usuario>();
                
                if (usuarios.Count == 0)
                {
                    return NotFound("No hay usuarios registrados en el sistema");
                }
                
                // Buscar un usuario con las credenciales proporcionadas
                var usuario = usuarios.FirstOrDefault(u => 
                    u.nombre_usuario == model.NombreUsuario && 
                    u.contrasena_hash == model.Contrasena);

                if (usuario != null)
                {
                    return Ok(usuario);
                }

                return Unauthorized("Credenciales inválidas");
            }
            catch (Exception ex)
            {
                // Registrar el error para depuración
                Console.WriteLine($"Error en login: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
