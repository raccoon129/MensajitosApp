using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMMON.Entidades
{
    public class Mensaje:CamposControl
    {
        public int id_mensaje { get; set; }
        public int emisor_id { get; set; }
        public int receptor_id { get; set; }
        public string contenido { get; set; } 

    }
}
