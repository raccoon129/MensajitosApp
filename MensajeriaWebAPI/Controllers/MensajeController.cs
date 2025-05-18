using Microsoft.AspNetCore.Components;
using COMMON.Entidades;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace MensajeriaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajeController : GenericController<Mensaje>
    {
        public MensajeController() : base(Parametros.FabricaRepository.MensajeRepository())
        {

        }

        // Método para obtener mensajes de un usuario específico
        [HttpGet("usuario/{idUsuario}")]
        public ActionResult<List<Mensaje>> GetMensajesPorUsuario(int idUsuario)
        {
            try
            {
                // Obtener todos los mensajes
                var todosMensajes = _repositorio.ObtenerTodas();

                if (todosMensajes == null)
                {
                    return BadRequest(_repositorio.Error);
                }

                // Filtrar solo los mensajes donde el usuario es emisor o receptor
                var mensajesFiltrados = todosMensajes.Where(m =>
                    m.emisor_id == idUsuario || m.receptor_id == idUsuario)
                    .OrderBy(m => m.fecha_ejec)
                    .ToList();

                return Ok(mensajesFiltrados);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Método para obtener conversación entre dos usuarios
        [HttpGet("conversacion")]
        public ActionResult<List<Mensaje>> GetConversacion([FromQuery] int usuario1, [FromQuery] int usuario2)
        {
            try
            {
                // Obtener todos los mensajes
                var todosMensajes = _repositorio.ObtenerTodas();

                if (todosMensajes == null)
                {
                    return BadRequest(_repositorio.Error);
                }

                // Filtrar para obtener solo la conversación entre los dos usuarios
                var conversacion = todosMensajes.Where(m =>
                    (m.emisor_id == usuario1 && m.receptor_id == usuario2) ||
                    (m.emisor_id == usuario2 && m.receptor_id == usuario1))
                    .OrderBy(m => m.fecha_ejec)
                    .ToList();

                return Ok(conversacion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}