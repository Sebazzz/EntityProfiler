using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using EntityProfiler.Viewer.Properties;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

namespace EntityProfiler.Viewer.Modules.CodeEditor.Controls
{
    public class CodeEditor : TextEditor
    {
        public static readonly Color DefaultBackground = Color.FromArgb(255, 255, 255, 224);
        public static readonly Color DefaultBorder = Color.FromArgb(255, 234, 234, 242);

        public CodeEditor()
        {
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

            var profilerToolsSqlEditorOptions = Settings.Default.TextEditor_CodeEditorOptions;
            profilerToolsSqlEditorOptions.SetBindings(this);
        }


        #region CurrentLine

        public static readonly DependencyProperty CurrentLineBackgroundProperty =
            DependencyProperty.Register(
                "CurrentLineBackground", typeof(Brush), typeof(TextEditor),
                new PropertyMetadata(new SolidColorBrush(DefaultBackground)));

        public Brush CurrentLineBackground
        {
            get { return (Brush)GetValue(CurrentLineBackgroundProperty); }
            set { SetValue(CurrentLineBackgroundProperty, value); }
        }

        public static readonly DependencyProperty CurrentLineBorderProperty =
            DependencyProperty.Register(
                "CurrentLineBorder", typeof(Pen), typeof(TextEditor),
                new PropertyMetadata(new Pen(new SolidColorBrush(DefaultBorder), 1)));

        public Pen CurrentLineBorder
        {
            get { return (Pen)GetValue(CurrentLineBorderProperty); }
            set { SetValue(CurrentLineBorderProperty, value); }
        }

        #endregion
    }
}