using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMMON.Entidades;
using FluentValidation;

namespace COMMON.Validadores
{
    public class UsuarioValidator : CamposControlValidator<Usuario>
    {
        public UsuarioValidator()
        {
            RuleFor(c => c.id_usuario).NotEmpty();
            RuleFor(c => c.nombre_usuario).NotEmpty().MinimumLength(3);
            RuleFor(c => c.contrasena_hash).NotEmpty().MinimumLength(5);
        }
    }
    
}
