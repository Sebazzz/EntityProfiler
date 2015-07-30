using System.Globalization;
using System.Windows;

namespace Gemini.Framework
{
    public class NullableBooleanToVisibilityConverter : ConverterBase<bool?, Visibility>
    {
        public NullableBooleanToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
            NullValue = Visibility.Collapsed;
        }

        public override Visibility Convert(bool? value, CultureInfo culture)
        {
            if (!value.HasValue)
                return NullValue;

            return !value.Value ? FalseValue : TrueValue;
        }

        public Visibility NullValue { get; set; }

        public Visibility FalseValue { get; set; }

        public Visibility TrueValue { get; set; }
    }
}