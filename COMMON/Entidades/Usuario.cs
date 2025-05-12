using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMMON.Entidades
{
    public class Usuario:CamposControl
    {
        public int id_usuario { get; set; }
        public string? nombre_usuario { get; set; }
        public string contrasena_hash { get; set; }
    }
}
