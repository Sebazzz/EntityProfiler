using System.Collections.Generic;
using ICSharpCode.AvalonEdit;

namespace EntityProfiler.Viewer.Modules.QueryTools.Views
{
    public interface IDatabaseQueryRunnerView
    {
        TextEditor TextEditor { get; }
    }
}