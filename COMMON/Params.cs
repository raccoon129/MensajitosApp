using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMMON
{
    public static class Params
    {
        public static string UsuarioConectado = "UsuarioDeMensajitos";

#if DEBUG
        public static string UrlAPI = @"http://localhost:5253/";
#else
        public static string UrlAPI = @"http://mensajitos.runasp.net/";
#endif
    }
}
