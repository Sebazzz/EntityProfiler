using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml;
using Caliburn.Micro;
using EntityProfiler.Common.Annotations;
using Gemini.Framework.Themes;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace EntityProfiler.Viewer.Modules.CodeEditor
{
    public class HighlightingThemeManager : INotifyPropertyChanged
    {
        private const string LightXshrResource = "EntityProfiler.Viewer.Modules.CodeEditor.SyntaxDefinitions.SQL.xshd";
        private const string DarkXshrResource = "EntityProfiler.Viewer.Modules.CodeEditor.SyntaxDefinitions.SQLdark.xshd";

        private static readonly Lazy<HighlightingThemeManager> _current =
            new Lazy<HighlightingThemeManager>(() => new HighlightingThemeManager());

        private readonly IThemeManager _themeManager;
        private IHighlightingDefinition _currentHighlightingDefinition;
        private bool _invalidateCurrentHighlighting;
        private string _currentThemeName;

        private HighlightingThemeManager()
        {
            _themeManager = IoC.Get<IThemeManager>();
            _themeManager.CurrentThemeChanged += ThemeManagerOnCurrentThemeChanged;
            _currentThemeName = _themeManager.CurrentTheme.Name;
        }

        public static HighlightingThemeManager Current
        {
            get { return _current.Value; }
        }

        public IHighlightingDefinition ThemeHighlightingDefinition
        {
            get
            {
                if (_currentHighlightingDefinition == null || _invalidateCurrentHighlighting)
                {
                    _currentHighlightingDefinition =
                        _currentThemeName.Equals("Dark", StringComparison.InvariantCultureIgnoreCase) ?
                            LoadHighlightingFromAssembly(DarkXshrResource) : LoadHighlightingFromAssembly(LightXshrResource);
                    
                    _invalidateCurrentHighlighting = false;
                }
                return _currentHighlightingDefinition;
            }
        }

        private static IHighlightingDefinition LoadHighlightingFromAssembly(string name)
        {
            // https://edi.codeplex.com/SourceControl/latest#Edi/AvalonEdit/Highlighting/SQL.xshd
            using (var s = typeof(HighlightingThemeManager).Assembly.GetManifestResourceStream(name))
            {
                Debug.Assert(s != null);
                using (var reader = new XmlTextReader(s))
                {
                    return HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ThemeManagerOnCurrentThemeChanged(object sender, EventArgs eventArgs)
        {
            if (_currentThemeName == _themeManager.CurrentTheme.Name)
                return;

            _currentThemeName = _themeManager.CurrentTheme.Name;
            _invalidateCurrentHighlighting = true;
            OnPropertyChanged("ThemeHighlightingDefinition");
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}