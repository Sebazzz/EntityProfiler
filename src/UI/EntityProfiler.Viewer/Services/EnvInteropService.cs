using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;

namespace EntityProfiler.Viewer.Services
{
    public interface IEnvInteropService
    {
        IEnumerable<DTE> VisualStudioInstances { get; set; }
        bool TrySelectLineInsideVisualStudio(string sourceCodeFileFullPath, int line, int column, bool allowAnyInstance = true);
        void InvalidateVisualStudioInstances();
    }

    [Export(typeof (IEnvInteropService))]
    public class EnvInteropService : IEnvInteropService
    {
        // Source adapted from: http://www.codeproject.com/Articles/286265/Reduce-debugging-time-using-the-NET-StackTrace-cla

        private const string DefaultViewKind = Constants.vsViewKindCode;

        private bool _retryGetVisualStudioInstances;
        private IEnumerable<DTE> _dtes;

        public IEnumerable<DTE> VisualStudioInstances
        {
            get { return _dtes ?? (_dtes = GetVisualStudioInstances()); }
            set { _dtes = value; }
        }

        public bool TrySelectLineInsideVisualStudio(string sourceCodeFileFullPath, int line, int column, bool allowAnyInstance = true)
        {
            if (!_retryGetVisualStudioInstances &&
                (string.IsNullOrEmpty(sourceCodeFileFullPath) || !File.Exists(sourceCodeFileFullPath))) return false;

            try
            {
                if (VisualStudioInstances != null && VisualStudioInstances.Any())
                {
                    foreach (var visualStudioInstance in VisualStudioInstances)
                    {
                        // Open if instance has file loaded in project
                        var ptojItem = visualStudioInstance.Solution.FindProjectItem(sourceCodeFileFullPath);
                        if (null != ptojItem)
                        {
                            ptojItem.Open(DefaultViewKind).Activate();
                            var textSelection = (TextSelection) visualStudioInstance.ActiveDocument.Selection;
                            textSelection.MoveToLineAndOffset(line, column);
                            textSelection.SelectLine();
                            visualStudioInstance.MainWindow.Activate();
                            _retryGetVisualStudioInstances = false;
                            return true;
                        }
                    }
                }

                if (_retryGetVisualStudioInstances)
                {
                    _retryGetVisualStudioInstances = false;
                    if (allowAnyInstance && VisualStudioInstances != null && VisualStudioInstances.Any())
                    {
                        // Force open in avaliable instance
                        var visualStudioInstance = VisualStudioInstances.FirstOrDefault();
                        if (visualStudioInstance != null)
                        {
                            visualStudioInstance.OpenFile(DefaultViewKind, sourceCodeFileFullPath).Activate();
                            var textSelection = (TextSelection)visualStudioInstance.ActiveDocument.Selection;
                            textSelection.MoveToLineAndOffset(line, column);
                            textSelection.SelectLine();
                            visualStudioInstance.MainWindow.Activate();
                            return true;
                        }
                    }
                    return false;
                }

                _retryGetVisualStudioInstances = true;
                InvalidateVisualStudioInstances();
                return TrySelectLineInsideVisualStudio(sourceCodeFileFullPath, line, column, allowAnyInstance);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                if (null != exception.InnerException)
                    Debug.WriteLine(exception.InnerException.Message);
            }

            _retryGetVisualStudioInstances = false;
            return false;
        }

        public void InvalidateVisualStudioInstances()
        {
            _dtes = null;
        }

        private IEnumerable<DTE> GetVisualStudioInstances()
        {
            var result = new HashSet<DTE>();
            // Get an instance of the currently running Visual Studio IDE
            var sDTENameVS2015 = "VisualStudio.DTE.14.0"; //- For Visual Studio 2015
            var sDTENameVS2012 = "VisualStudio.DTE.12.0"; //- For Visual Studio 2013
            var sDTENameVS2010 = "VisualStudio.DTE.10.0"; //- For Visual Studio 2010
            var sDTENameVS2008 = "VisualStudio.DTE.9.0"; //- For Visual Studio 2008
            var sDTENameVS2005 = "VisualStudio.DTE.8.0"; //- For Visual Studio 2005
            var sDTENameVS2003 = "VisualStudio.DTE.7.1"; //- For Visual Studio.NET 2003
            var sDTENameVS2002 = "VisualStudio.DTE.7"; //- For Visual Studio.NET 2002
            string[] dtes =
            {
                sDTENameVS2015, sDTENameVS2012, sDTENameVS2010, sDTENameVS2008, sDTENameVS2005,
                sDTENameVS2003, sDTENameVS2002
            };

            foreach (var it in dtes)
            {
                try
                {
                    var dte = (DTE) Marshal.GetActiveObject(it);
                    if (null != dte)
                        result.Add(dte);
                }
                catch (Exception ex)
                {
                    //- try another VS
                    Debug.WriteLine(ex.Message);
                }
            }
            return result;
        }
    }
}