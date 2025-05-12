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
            RuleFor(c => c.emisor_id).NotEmpty();
            RuleFor(c => c.receptor_id).NotEmpty();
            RuleFor(c => c.receptor_id).NotEmpty();
            RuleFor(c => c.contenido).NotEmpty().MinimumLength(1);

        }
    }
}
