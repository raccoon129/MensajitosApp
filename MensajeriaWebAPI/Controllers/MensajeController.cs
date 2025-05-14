using Microsoft.AspNetCore.Components;
using COMMON.Entidades;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace MensajeriaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MensajeController:GenericController<Mensaje>
    {
        public MensajeController(): base (Parametros.FabricaRepository.MensajeRepository())
        {
            
        }
    }
}
