using System.Globalization;
using System.Windows;
using Gemini.Framework;

namespace EntityProfiler.Viewer.PresentationCore
{
    public abstract class VisibilityToValueConverter<TTo> :
        ConverterBase<Visibility, TTo>
    {
        #region Overrides

        public override TTo Convert(Visibility value, CultureInfo culture)
        {
            switch (value)
            {
                case Visibility.Visible:
                    return VisibleValue;
                default:
                    return CollapsedValue;
            }
        }

        #endregion

        #region Properties

        public TTo CollapsedValue { get; set; }

        public TTo VisibleValue { get; set; }

        #endregion
    }
}