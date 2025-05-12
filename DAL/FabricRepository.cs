using COMMON.Entidades;
using COMMON.Interfaces;
using COMMON.Validadores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class FabricRepository
    {
        private string _cadenaDeConexion;
        private TipoDB _tipoDB;

        public FabricRepository(string cadenaDeConexion, TipoDB tipoDB)
        {
            _cadenaDeConexion = cadenaDeConexion;
            _tipoDB = tipoDB;
        }

        public IDB<Usuario> UsuarioRepository()
        {
            switch (_tipoDB)
            {
                case TipoDB.MySQL:
                    return new MySQL<Usuario>(_cadenaDeConexion, new UsuarioValidator(), "id_usuario", false);
                default:
                    throw new NotImplementedException("Tipo de base de datos no soportado");
            }
        }

        public IDB<Mensaje> MensajeRepository()
        {
            switch (_tipoDB)
            {
                case TipoDB.MySQL:
                    return new MySQL<Mensaje>(_cadenaDeConexion, new MensajeValidator(), "id_mensaje", true);
                default:
                    throw new NotImplementedException("Tipo de base de datos no soportado");
            }
        }
    }

    public enum TipoDB
    {
        MySQL
    }
}