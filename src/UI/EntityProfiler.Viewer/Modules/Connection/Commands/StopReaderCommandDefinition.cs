using System;
using System.Windows.Input;
using Gemini.Framework.Commands;

namespace EntityProfiler.Viewer.Modules.Connection
{
    [CommandDefinition]
    public class StopReaderCommandDefinition : CommandDefinition
    {
        public override string Name
        {
            get { return "Interceptor.StopReader"; }
        }

        public override string Text
        {
            get { return "Stop"; }
        }

        public override string ToolTip
        {
            get { return "Stop interceptor reader"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Resources/images/Stop_16xLG_color.png", UriKind.Absolute); }
        }

        public override KeyGesture KeyGesture
        {
            get { return new KeyGesture(Key.F5, ModifierKeys.Shift); }
        }
    }
}