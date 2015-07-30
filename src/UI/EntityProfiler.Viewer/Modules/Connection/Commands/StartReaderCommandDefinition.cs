using System;
using System.Windows.Input;
using Gemini.Framework.Commands;

namespace EntityProfiler.Viewer.Modules.Connection
{
    [CommandDefinition]
    public class StartReaderCommandDefinition : CommandDefinition
    {
        public override string Name
        {
            get { return "Interceptor.StartReader"; }
        }

        public override string Text
        {
            get { return "Start"; }
        }

        public override string ToolTip
        {
            get { return "Start interceptor reader"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Resources/images/Running_16xLG.png", UriKind.Absolute); }
        }

        public override KeyGesture KeyGesture
        {
            get { return new KeyGesture(Key.F5); }
        }
    }
}