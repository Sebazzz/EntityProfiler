using System.Globalization;
using System.Windows;

namespace Gemini.Framework
{
    public class BooleanToVisibilityConverter : ConverterBase<bool, Visibility>
    {
        public BooleanToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public override Visibility Convert(bool value, CultureInfo culture)
        {
            return !value ? FalseValue : TrueValue;
        }

        public Visibility FalseValue { get; set; }

        public Visibility TrueValue { get; set; }
    }
}