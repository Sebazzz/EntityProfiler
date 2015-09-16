using System;
using System.Windows.Input;
using Gemini.Framework.Commands;

namespace EntityProfiler.Viewer.Modules.Connection
{
    [CommandDefinition]
    public class PauseReaderCommandDefinition : CommandDefinition
    {
        public override string Name
        {
            get { return "Interceptor.PauseReader"; }
        }

        public override string Text
        {
            get { return "Pause"; }
        }

        public override string ToolTip
        {
            get { return "Pause interceptor reader"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Resources/images/Pause_16xLG_color.png", UriKind.Absolute); }
        }

        public override KeyGesture KeyGesture
        {
            get { return new KeyGesture(Key.Pause); }
        }
    }
}