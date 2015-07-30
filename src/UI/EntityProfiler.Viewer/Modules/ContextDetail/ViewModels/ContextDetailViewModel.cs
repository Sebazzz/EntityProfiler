using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Caliburn.Micro;
using EntityProfiler.Common.Protocol;
using EntityProfiler.Viewer.Modules.CodeEditor;
using EntityProfiler.Viewer.Modules.Connection;
using EntityProfiler.Viewer.Modules.Connection.ViewModels;
using EntityProfiler.Viewer.Modules.QueryTools.ViewModels;
using EntityProfiler.Viewer.Properties;
using EntityProfiler.Viewer.Services;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace EntityProfiler.Viewer.Modules.ContextDetail
{
    [Export]
    public class ContextDetailViewModel : Document
    {
        private readonly IEnvInteropService _envInteropService;
        private readonly ICodeEditorFactory _codeEditorFactory;
        private bool _hasParameters;
        private IContextDetailView _view;
        private Record _selectedParametersRecord;

        [ImportingConstructor]
        public ContextDetailViewModel(IEnvInteropService envInteropService, ICodeEditorFactory codeEditorFactory)
        {
            _envInteropService = envInteropService;
            _codeEditorFactory = codeEditorFactory;
        }

        public override string DisplayName
        {
            get { return "Context detail"; }
        }

        public string CommandText
        {
            get
            {
                if (SessionData.Current.SelectedQuery != null && SessionData.Current.SelectedQuery.Model != null &&
                    SessionData.Current.SelectedQuery.Model.Query != null)
                    return SessionData.Current.SelectedQuery.Model.Query.CommandText;
                return null;
            }
        }

        public Record SelectedParametersRecord
        {
            get { return _selectedParametersRecord; }
            set
            {
                if (Equals(value, _selectedParametersRecord)) return;
                _selectedParametersRecord = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange("SelectedRecordLookup");
            }
        }

        public IDictionary<string, object> SelectedRecordLookup
        {
            get { return GetRecordLookup(SelectedParametersRecord); }
        }

        public bool HasParameters
        {
            get { return _hasParameters; }
            private set
            {
                if (value == _hasParameters) return;
                _hasParameters = value;
                NotifyOfPropertyChange();
            }
        }

        public string ConnectionString
        {
            get
            {
                if(SessionData.Current.SelectedQuery == null || SessionData.Current.SelectedQuery.Model == null)
                    return string.Empty;
                object result;
                SessionData.Current.SelectedQuery.Model.Context.Values.TryGetValue("ConnectionString", out result);
                return result != null ? result.ToString() : string.Empty;
            }
        }

        public void OpenInQueryRunner()
        {
            var shell = IoC.Get<IShell>();
            var databaseQuery = IoC.Get<DatabaseQueryRunnerViewModel>();
            var commandText = _view.TextEditor.Text;
            databaseQuery.SetNewQuery(ConnectionString, commandText);
            shell.OpenDocument(databaseQuery);
        }

        public bool CanOpenInQueryRunner
        {
            get { return !string.IsNullOrEmpty(ConnectionString) && !string.IsNullOrEmpty(CommandText); }
        }

        public void CopyConnectionString()
        {
            var connectionString = ConnectionString;
            if(!string.IsNullOrEmpty(connectionString))
                Clipboard.SetText(connectionString);
        }

        public bool CanCopyConnectionString
        {
            get { return !string.IsNullOrEmpty(ConnectionString); }
        }

        public void OpenStackFrame(StackFrame stackFrame)
        {
            if (stackFrame != null)
            {
                var sourceCodeFileFullPath = stackFrame.FilePath;
                if (string.IsNullOrEmpty(sourceCodeFileFullPath) || !File.Exists(sourceCodeFileFullPath))
                {
                    MessageBox.Show(String.Format("Could not find the file {0}.", sourceCodeFileFullPath), "File not found");
                    return;
                }
                var opened = false;
                var openFilesInVisualStudio = Settings.Default.Profiler_CallStack_OpenFilesInVisualStudio;
                if (openFilesInVisualStudio)
                {
                    var allowAnyVsInstance = Settings.Default.Profiler_CallStack_AllowAnyVsInstance;
                    opened = _envInteropService.TrySelectLineInsideVisualStudio(
                        sourceCodeFileFullPath,
                        stackFrame.LineNumber,
                        stackFrame.ColumnNumber,
                        allowAnyVsInstance);
                }
                if (!opened)
                {
                    _codeEditorFactory.Open(sourceCodeFileFullPath, editor =>
                    {
                        editor.TextEditor.ScrollTo(stackFrame.LineNumber, stackFrame.ColumnNumber);
                        editor.TextEditor.TextArea.Caret.Line = stackFrame.LineNumber;
                        editor.TextEditor.TextArea.Caret.Column = stackFrame.ColumnNumber;
                        editor.TextEditor.Focus();
                    });
                }
            }
        }

        private void CurrentOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SelectedQuery"))
            {
                UpdateEditor();
                UpdateParamentersDataGrid();
                NotifyOfPropertyChange(()=> ConnectionString);
                NotifyOfPropertyChange(()=> CanCopyConnectionString);
                NotifyOfPropertyChange(()=> CanOpenInQueryRunner);
            }
        }

        protected override void OnActivate()
        {
            SessionData.Current.PropertyChanged += CurrentOnPropertyChanged;
        }

        protected override void OnDeactivate(bool close)
        {
            SessionData.Current.PropertyChanged -= CurrentOnPropertyChanged;
        }

        protected override void OnViewLoaded(object view)
        {
            _view = (IContextDetailView) view;
            UpdateEditor();
            UpdateParamentersDataGrid();
        }

        private static IDictionary<string, object> GetRecordLookup(Record record)
        {
            if(record == null)
                return null;

            return record.Properties
                .Where(p => !p.Name.StartsWith("#"))
                .Aggregate(new Dictionary<string, object>(), (list, item) =>
                {
                    if (item.Name.StartsWith("@"))
                    {
                        list.Add(item.Name, item.Value);
                        return list;
                    }

                    if (item.Name.Equals("Value"))
                    {
                        if (list.ContainsKey("@Name"))
                        {
                            list.Add(list["@Name"].ToString(), item.Value);
                            list.Remove("@Name");
                        }
                        return list;
                    }

                    list.Add("@" + item.Name, item.Value);

                    return list;
                });
        }

        private void UpdateEditor()
        {
            NotifyOfPropertyChange(() => CommandText);
        }

        private void UpdateParamentersDataGrid()
        {
            var query = SessionData.Current.SelectedQuery;
            if (query == null)
            {
                return;
            }

            var dataGrid = _view.DataGrid;
            var first = query.Parameters.FirstOrDefault();
            dataGrid.Columns.Clear();
            if (first == null)
            {
                HasParameters = false;
                dataGrid.ItemsSource = null;
                return;
            }

            _selectedParametersRecord = first;

            var columns = first
                .Properties
                .Select((x, i) => new {x.Name, Index = i})
                .ToArray();

            foreach (var column in columns)
            {
                var binding = new Binding(string.Format("Properties[{0}].Value", column.Index))
                {
                    Mode = BindingMode.OneTime
                };
                dataGrid.Columns.Add(new DataGridTextColumn {Header = column.Name, Binding = binding});
            }

            dataGrid.ItemsSource = query.Parameters;
            HasParameters = true;
            NotifyOfPropertyChange("SelectedParametersRecord");
        }
    }
}