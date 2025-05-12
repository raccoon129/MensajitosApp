using Microsoft.AspNetCore.Components;
using COMMON.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace MensajeriaWebAPI.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]

    public class MensajeController:GenericController<Mensaje>
    {
        public MensajeController(): base (Parametros.FabricaRepository.MensajeRepository())
        {
            
        }
    }
}
