using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using EntityProfiler.Viewer.Modules.ContextDetail.Controls;
using EntityProfiler.Viewer.Properties;
using EntityProfiler.Viewer.Services;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

namespace EntityProfiler.Viewer.Modules.CodeEditor.Controls
{
    public class SqlEditor : TextEditor
    {
        public static readonly Color DefaultBackground = Color.FromArgb(255, 255, 255, 224);
        public static readonly Color DefaultBorder = Color.FromArgb(255, 234, 234, 242);

        static SqlEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SqlEditor), new FrameworkPropertyMetadata(typeof(SqlEditor)));
        }

        public SqlEditor()
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            
            SetBindings();
        }

        private void SetBindings()
        {
            TextArea.TextView.SetBinding(TextView.CurrentLineBackgroundProperty, new Binding("CurrentLineBackground")
            {
                Mode = BindingMode.OneWay,
                Source = this
            });

            TextArea.TextView.SetBinding(TextView.CurrentLineBorderProperty, new Binding("CurrentLineBorder")
            {
                Mode = BindingMode.OneWay,
                Source = this
            });

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            var profilerToolsSqlEditorOptions = Settings.Default.TextEditor_SqlEditorOptions;
            profilerToolsSqlEditorOptions.SetBindings(this);

            SetBinding(SyntaxHighlightingProperty, new Binding("ThemeHighlightingDefinition")
            {
                Source = HighlightingThemeManager.Current,
                Mode = BindingMode.OneWay
            });
        }

        protected virtual void SetCommandText(string newValue, string oldValue)
        {
            if (oldValue != null && newValue == oldValue)
                return;

            Text = newValue;
        }

        private const string ParameterRelplaceFormat = @"{2}{0}{2} /*{1}*/";

        protected virtual void SetCommandParameters(IDictionary<string, object> newValue, IDictionary<string, object> oldValue)
        {
            var commandText = CommandText;
            if (newValue == null || newValue.Count == 0)
            {
                if (oldValue != null)
                    SetCommandText(commandText, null);
                return;
            }
            
            foreach (var prop in newValue)
            {
                string valueWapper;
                if (!DotnetTypeMap.TypeMapValueWapper.TryGetValue(prop.Value.GetType(), out valueWapper))
                    valueWapper = DotnetTypeMap.DefaultValueWapper;
                
                var paramText = string.Format(ParameterRelplaceFormat, prop.Value, prop.Key, valueWapper);
                commandText = commandText.Replace(prop.Key, paramText);
            }
            SetCommandText(commandText, CommandText);
        }

        #region CurrentLine

        public static readonly DependencyProperty CurrentLineBackgroundProperty =
            DependencyProperty.Register(
                "CurrentLineBackground", typeof (Brush), typeof (SqlEditor),
                new PropertyMetadata(new SolidColorBrush(DefaultBackground)));

        public Brush CurrentLineBackground
        {
            get { return (Brush) GetValue(CurrentLineBackgroundProperty); }
            set { SetValue(CurrentLineBackgroundProperty, value); }
        }

        public static readonly DependencyProperty CurrentLineBorderProperty =
            DependencyProperty.Register(
                "CurrentLineBorder", typeof (Pen), typeof (SqlEditor),
                new PropertyMetadata(new Pen(new SolidColorBrush(DefaultBorder), 1)));

        public Pen CurrentLineBorder
        {
            get { return (Pen) GetValue(CurrentLineBorderProperty); }
            set { SetValue(CurrentLineBorderProperty, value); }
        }

        #endregion
        
        #region CommandText

        public static readonly DependencyProperty CommandTextProperty =
            DependencyProperty.Register(
                "CommandText", typeof (string), typeof (SqlEditor),
                new PropertyMetadata(default(string), OnCommandTextChanged));

        public string CommandText
        {
            get { return (string) GetValue(CommandTextProperty); }
            set { SetValue(CommandTextProperty, value); }
        }

        public static readonly RoutedEvent CommandTextChangedEvent =
            EventManager.RegisterRoutedEvent(
                "CommandTextChanged",
                RoutingStrategy.Bubble,
                typeof (RoutedPropertyChangedEventHandler<string>),
                typeof (SqlEditor));

        public event RoutedPropertyChangedEventHandler<string> CommandTextChanged
        {
            add { AddHandler(CommandTextChangedEvent, value); }
            remove { RemoveHandler(CommandTextChangedEvent, value); }
        }

        private static void OnCommandTextChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (SqlEditor) d;
            var args = new RoutedPropertyChangedEventArgs<string>(
                (string) e.OldValue,
                (string) e.NewValue)
            {
                RoutedEvent = CommandTextChangedEvent
            };
            instance.RaiseEvent(args);
            instance.SetCommandText(args.NewValue, args.OldValue);
        }

        #endregion

        #region CommandParameters

        public static readonly DependencyProperty CommandParametersProperty =
            DependencyProperty.Register(
                "CommandParameters", typeof (IDictionary<string, object>), typeof (SqlEditor),
                new PropertyMetadata(default(IDictionary<string, object>), OnCommandParametersChanged));

        public IDictionary<string, object> CommandParameters
        {
            get { return (IDictionary<string, object>) GetValue(CommandParametersProperty); }
            set { SetValue(CommandParametersProperty, value); }
        }

        public static readonly RoutedEvent CommandParametersChangedEvent =
            EventManager.RegisterRoutedEvent(
                "CommandParametersChanged",
                RoutingStrategy.Bubble,
                typeof (RoutedPropertyChangedEventHandler<IDictionary<string, object>>),
                typeof (SqlEditor));

        public event RoutedPropertyChangedEventHandler<IDictionary<string, object>> CommandParametersChanged
        {
            add { AddHandler(CommandParametersChangedEvent, value); }
            remove { RemoveHandler(CommandParametersChangedEvent, value); }
        }

        private static void OnCommandParametersChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (SqlEditor) d;
            var args = new RoutedPropertyChangedEventArgs<IDictionary<string, object>>(
                (IDictionary<string, object>) e.OldValue,
                (IDictionary<string, object>) e.NewValue)
            {
                RoutedEvent = CommandParametersChangedEvent
            };
            instance.RaiseEvent(args);
            instance.SetCommandParameters(args.NewValue, args.OldValue);
        }

        #endregion
    }
}