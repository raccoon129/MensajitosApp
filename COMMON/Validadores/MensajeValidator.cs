using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMMON.Entidades;
using FluentValidation;

namespace COMMON.Validadores
{
    public class MensajeValidator:CamposControlValidator<Mensaje>
    {
        public MensajeValidator()
        {
            RuleFor(m => m.emisor_id)
                .NotEqual(0).WithMessage("El ID del emisor es requerido");

            RuleFor(m => m.receptor_id)
                .NotEqual(0).WithMessage("El ID del receptor es requerido");

            RuleFor(m => m.contenido)
                .NotEmpty().WithMessage("El contenido del mensaje es requerido")
                .MaximumLength(1000).WithMessage("El mensaje no puede exceder 1000 caracteres");
        }
    }
}
