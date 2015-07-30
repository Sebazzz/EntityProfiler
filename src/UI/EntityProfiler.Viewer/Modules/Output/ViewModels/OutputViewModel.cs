using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using EntityProfiler.Viewer.Modules.Output.Views;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace EntityProfiler.Viewer.Modules.Output.ViewModels
{
    [Export(typeof(IOutput))]
    public class OutputViewModel : Tool, IOutput
    {
        private readonly StringBuilder _stringBuilder;
        private readonly OutputWriter _writer;
        private IOutputView _view;
        private IDictionary<string, string> _outputSource;
        private string _selectedOutputSource;

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Bottom; }
        }

        public override bool ShouldReopenOnStart { get { return false; } }

        public TextWriter Writer
        {
            get { return _writer; }
        }

        public OutputViewModel()
        {
            DisplayName = "Output";

            _stringBuilder = new StringBuilder();
            _writer = new OutputWriter(this);

            OutputSource = new Dictionary<string, string>
            {
                {"app_logger","Application log" }
            };
        }

        public IDictionary<string, string> OutputSource
        {
            get { return _outputSource; }
            private set
            {
                _outputSource = value;
                if (string.IsNullOrEmpty(SelectedOutputSource) || !_outputSource.ContainsKey(SelectedOutputSource))
                    SelectedOutputSource = _outputSource.FirstOrDefault().Key;
            }
        }

        public string SelectedOutputSource
        {
            get { return _selectedOutputSource; }
            set
            {
                if (value == _selectedOutputSource) return;
                _selectedOutputSource = value;
                NotifyOfPropertyChange();
            }
        }

        public void Clear()
        {
            if (_view != null)
                Execute.OnUIThread(() => _view.Clear());
            _stringBuilder.Clear();
        }

        public void AppendLine(string text)
        {
            Append(text + Environment.NewLine);
        }

        public void Append(string text)
        {
            _stringBuilder.Append(text);
            OnTextChanged();
        }

        private void OnTextChanged()
        {
            if (_view != null)
                Execute.OnUIThread(() => _view.SetText(_stringBuilder.ToString()));
        }

        protected override void OnViewLoaded(object view)
        {
            _view = (IOutputView)view;
            _view.SetText(_stringBuilder.ToString());
            _view.ScrollToEnd();
        }
    }
}