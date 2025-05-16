using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensajeriaMAUI.Convertidores
{
    public class BoolToColorConverter : IValueConverter
    {
        //Obtiene colores a convertir
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Colors.Green : Colors.Gray;
        }

        //LO hace a la inverta pero queda pendiente
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
