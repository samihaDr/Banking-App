using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace BankApp.View {
    [ValueConversion(typeof(double?), typeof(string))]
    public class CurrencyToStringWithPlusSignConverter : MarkupExtension, IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value != null && (value is double || value is float)) {
                double val = (double)value;
                return (val > 0 ? "+" : "") + val.ToString("C2", culture);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            double? val = null;
            if (value != null && value is string str) {
                double tmp;
                if (double.TryParse(str, NumberStyles.Any, culture, out tmp))
                    val = tmp;
            }
            return val;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
