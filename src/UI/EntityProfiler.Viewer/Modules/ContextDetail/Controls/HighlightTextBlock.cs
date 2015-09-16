using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using EntityProfiler.Viewer.Modules.CodeEditor;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace EntityProfiler.Viewer.Modules.ContextDetail.Controls
{
    public class HighlightTextBlock : TextBlock
    {
        public HighlightTextBlock()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            SetBinding(SyntaxHighlightingProperty, new Binding("ThemeHighlightingDefinition")
            {
                Source = HighlightingThemeManager.Current,
                Mode = BindingMode.OneWay
            });
        }

        // See: http://stackoverflow.com/questions/21558386/avalonedit-getting-syntax-highlighted-text
        public void ApplyHighlighter(string code)
        {
            Inlines.Clear();
            if (string.IsNullOrEmpty(code))
                return;

            var item = this;

            var document = new TextDocument(code);
            var highlighter = new DocumentHighlighter(document, SyntaxHighlighting);
            var lineCount = document.LineCount;

            for (var lineNumber = 1; lineNumber <= document.LineCount; lineNumber++)
            {
                var line = highlighter.HighlightLine(lineNumber);

                var lineText = document.GetText(line.DocumentLine);

                var offset = line.DocumentLine.Offset;

                var sectionCount = line.Sections.Count;
                for (var sectionNumber = 0; sectionNumber < sectionCount; sectionNumber++)
                {
                    var section = line.Sections[sectionNumber];

                    //Deal with previous text
                    if (section.Offset > offset)
                    {
                        item.Inlines.Add(new Run(document.GetText(offset, section.Offset - offset)));
                    }

                    var runItem = new Run(document.GetText(section));

                    if (runItem.Foreground != null)
                    {
                        runItem.Foreground = section.Color.Foreground.GetBrush(null);
                    }
                    if (section.Color.FontWeight != null)
                    {
                        runItem.FontWeight = section.Color.FontWeight.Value;
                    }

                    item.Inlines.Add(runItem);

                    offset = section.Offset + section.Length;
                }

                //Deal with stuff at end of line
                var lineEnd = line.DocumentLine.Offset + lineText.Length;
                if (lineEnd > offset)
                {
                    item.Inlines.Add(new Run(document.GetText(offset, lineEnd - offset)));
                }

                //If not last line add a new line
                if (lineNumber < lineCount)
                {
                    item.Inlines.Add(new Run("\n"));
                }
            }
        }

        #region HighlightText property

        /// <summary>
        ///     HighlightText.
        /// </summary>
        public string HighlightText
        {
            get { return (string)GetValue(HighlightTextProperty); }
            set { SetValue(HighlightTextProperty, value); }
        }

        /// <summary>
        ///     Dependency property of HighlightText.
        /// </summary>
        public static readonly DependencyProperty HighlightTextProperty = DependencyProperty.Register("HighlightText",
            typeof(string), typeof(HighlightTextBlock), new FrameworkPropertyMetadata(null, OnHighlightTextChanged));

        /// <summary>
        ///     Occurs when the HighlightText property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> HighlightTextChanged;

        /// <summary>
        ///     Identifies the HighlightTextChanged routed event.
        /// </summary>
        public static readonly RoutedEvent HighlightTextChangedEvent =
            EventManager.RegisterRoutedEvent("HighlightTextChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<string>), typeof(HighlightTextBlock));

        /// <summary>
        ///     Raised when any of the instances HighlightText is changed.
        /// </summary>
        private static void OnHighlightTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var control = (HighlightTextBlock)obj;
            var e = new RoutedPropertyChangedEventArgs<string>((string)args.OldValue, (string)args.NewValue,
                HighlightTextChangedEvent);

            // Raises the controls protected OnHighlightTextChanged
            control.OnHighlightTextChanged(e);
        }

        /// <summary>
        ///     Raises the HighlightTextChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the HighlightTextChanged event.</param>
        protected virtual void OnHighlightTextChanged(RoutedPropertyChangedEventArgs<string> args)
        {
            // Raise the event in this instance.
            if (HighlightTextChanged != null)
            {
                HighlightTextChanged(this, args);
            }

            if (args != null && args.NewValue != null)
            {
                ApplyHighlighter(args.NewValue);
                return;
            }

            ApplyHighlighter(null);
        }

        #endregion

        #region SyntaxHighlighting property
        /// <summary>
        /// SyntaxHighlighting.
        /// </summary>
        public IHighlightingDefinition SyntaxHighlighting
        {
            get { return (IHighlightingDefinition)GetValue(SyntaxHighlightingProperty); }
            set { SetValue(SyntaxHighlightingProperty, value); }
        }

        /// <summary>
        /// Dependency property of SyntaxHighlighting.
        /// </summary>
        public static readonly DependencyProperty SyntaxHighlightingProperty = DependencyProperty.Register("SyntaxHighlighting", typeof(IHighlightingDefinition), typeof(HighlightTextBlock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSyntaxHighlightingChanged)));

        /// <summary>
        /// Occurs when the SyntaxHighlighting property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<IHighlightingDefinition> SyntaxHighlightingChanged;

        /// <summary>
        /// Identifies the SyntaxHighlightingChanged routed event.
        /// </summary>
        public static readonly RoutedEvent SyntaxHighlightingChangedEvent = EventManager.RegisterRoutedEvent("SyntaxHighlightingChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<IHighlightingDefinition>), typeof(HighlightTextBlock));

        /// <summary>
        /// Raised when any of the instances SyntaxHighlighting is changed.
        /// </summary>
        private static void OnSyntaxHighlightingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            HighlightTextBlock control = (HighlightTextBlock)obj;
            RoutedPropertyChangedEventArgs<IHighlightingDefinition> e = new RoutedPropertyChangedEventArgs<IHighlightingDefinition>((IHighlightingDefinition)args.OldValue, (IHighlightingDefinition)args.NewValue, SyntaxHighlightingChangedEvent);

            // Raises the controls protected OnSyntaxHighlightingChanged
            control.OnSyntaxHighlightingChanged(e);
        }

        /// <summary>
        /// Raises the SyntaxHighlightingChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the SyntaxHighlightingChanged event.</param>
        protected virtual void OnSyntaxHighlightingChanged(RoutedPropertyChangedEventArgs<IHighlightingDefinition> args)
        {
            // Raise the event in this instance.
            if (SyntaxHighlightingChanged != null)
                SyntaxHighlightingChanged(this, args);

            var code = GetValue(HighlightTextProperty) as string;
            ApplyHighlighter(code);
        }
        #endregion

    }
}