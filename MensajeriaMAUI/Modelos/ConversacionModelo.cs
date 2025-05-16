using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensajeriaMAUI.Modelos
{
    public class ConversacionModelo
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = "Alguien";
        public string? UltimoMensaje { get; set; }
        public string? TiempoUltimoMensaje { get; set; }
        public int? MensajesNoLeidos { get; set; } = 0;
        public bool? TieneMensajesNoLeidos { get; set; }

    }
}
