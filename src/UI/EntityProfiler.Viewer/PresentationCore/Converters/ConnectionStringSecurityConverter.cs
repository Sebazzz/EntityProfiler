using System.Globalization;
using Gemini.Framework;

namespace EntityProfiler.Viewer.PresentationCore
{
    public class ConnectionStringSecurityConverter : ConverterBase<string, string>
    {
        public override string Convert(string value, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.RemoveConnectionStringSecurity();
        }
    }
}