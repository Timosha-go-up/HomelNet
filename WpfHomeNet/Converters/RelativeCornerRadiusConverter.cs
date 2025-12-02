using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfHomeNet.Converters
{
    public class RelativeCornerRadiusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Ожидаем, что value — это Size (ширина и высота)
            if (value is Size size)
            {
                double minDimension = Math.Min(size.Width, size.Height); // Берём меньший размер
                double factor = parameter is double p ? p : 0.08; // 8% по умолчанию

                // Радиус не больше 8% от меньшего размера, минимум — 4 px
                return new CornerRadius(Math.Max(4, minDimension * factor));
            }

            return new CornerRadius(4); // Значение по умолчанию при ошибке
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }


}
