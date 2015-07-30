using System.Globalization;
using Gemini.Framework;

namespace EntityProfiler.Viewer.PresentationCore
{
    public abstract class BooleanToValueConverter<TTo> :
        ConverterBase<bool, TTo>
    {
        #region Overrides

        public override TTo Convert(bool value, CultureInfo culture)
        {
            return value ? TrueValue : FalseValue;
        }

        #endregion

        #region Properties

        public TTo FalseValue { get; set; }

        public TTo TrueValue { get; set; }

        #endregion
    }
}