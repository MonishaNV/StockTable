
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StockUi.Converters
{
    public class StockChangeToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 3 || !(values[0] is decimal) || !(values[1] is Brush) || !(values[2] is Brush))
            {
                return Brushes.Black;
            }
            decimal stockChange = (decimal)values[0];
            Brush raisingColor = (Brush)values[1];
            Brush fallingColor = (Brush)values[2];
            if (stockChange > 0)
            {
                return raisingColor;
            }
            else if (stockChange < 0)
            {
                return fallingColor;
            }
            else
            {
                return Brushes.Black;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}