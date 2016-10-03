using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sample.Wpf.Presentation.Core
{
    /// <summary>
    /// If used on multiple values acts as a Not(And(values)), so values True, False will be And'd
    /// to produce False and then Not'd to produce True
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class NotBooleanConverter : MarkupExtension,
        IValueConverter, IMultiValueConverter
    {
        public NotBooleanConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is bool && !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is bool && !(bool)value;
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return false;

            var booleans = values.Where(_ => _ is Boolean).ToArray();
            return !booleans.All(_ => (bool)_);
        }

        [ExcludeFromCodeCoverage]
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

}
