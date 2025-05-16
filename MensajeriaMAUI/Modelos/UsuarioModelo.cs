using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensajeriaMAUI.Modelos
{
    public class UsuarioModelo
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public bool IsOnLine { get; set; } //Para saber si está conectado
    }
}
