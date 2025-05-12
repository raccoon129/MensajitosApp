using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMMON.Entidades;
using FluentValidation;

namespace COMMON.Validadores
{
    public abstract class CamposControlValidator<T>:AbstractValidator<T> where T : CamposControl
    {
        public CamposControlValidator()
        {

        }
    }
}
