using System.Globalization;

namespace MyApp.Converters
{
    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAvailable)
            {
                return isAvailable ? "Disponible" : "Indisponible";
            }
            return "Statut inconnu";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}