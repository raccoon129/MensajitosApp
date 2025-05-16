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
            RuleFor(u => u.nombre_usuario)
                .NotEmpty().WithMessage("El nombre de usuario es requerido")
                .MinimumLength(3).WithMessage("El nombre de usuario debe tener al menos 3 caracteres")
                .MaximumLength(50).WithMessage("El nombre de usuario no puede exceder 50 caracteres");

            RuleFor(u => u.contrasena_hash)
                .NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");
        }
    }
    
}
