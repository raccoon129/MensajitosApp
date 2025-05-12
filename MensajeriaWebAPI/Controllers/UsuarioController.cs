using COMMON.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MensajeriaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController:GenericController<Usuario>
    {
        public UsuarioController():base(Parametros.FabricaRepository.UsuarioRepository())
        {
            
        }
    }
}
