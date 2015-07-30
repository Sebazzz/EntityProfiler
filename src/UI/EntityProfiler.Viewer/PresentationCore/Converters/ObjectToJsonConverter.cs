using System.Globalization;
using System.Threading.Tasks;
using Gemini.Framework;
using Newtonsoft.Json;

namespace EntityProfiler.Viewer.PresentationCore
{
    public class ObjectToJsonConverter : ConverterBase<object, string>
    {
        public override string Convert(object value, CultureInfo culture)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }
    }
}