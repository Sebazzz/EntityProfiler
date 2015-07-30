using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Serialization;
using EntityProfiler.Common.Annotations;
using ICSharpCode.AvalonEdit;
using PropertyTools.DataAnnotations;

namespace EntityProfiler.Viewer.PresentationCore
{
    [Serializable]
    [System.ComponentModel.DisplayName("Editor Options")]
    public partial class ExtendedTextEditorOptions : TextEditorOptions
    {
        private string _fontFamilyName = "Consolas";
        private double _fontSize = 12;
        private List<double> _fontSizes;
        private bool _showLineNumbers = true;

        public ExtendedTextEditorOptions()
        {
            HighlightCurrentLine = true;
            ConvertTabsToSpaces = true;
        }

        public ExtendedTextEditorOptions(ExtendedTextEditorOptions options)
            : base(options)
        {
            // get all the fields in the class
            var fields =
                typeof (ExtendedTextEditorOptions).GetFields(BindingFlags.NonPublic | BindingFlags.Public |
                                                             BindingFlags.Instance);

            // copy each value over to 'this'
            foreach (var fi in fields)
            {
                if (!fi.IsNotSerialized)
                    fi.SetValue(this, fi.GetValue(options));
            }
        }

        [System.ComponentModel.DisplayName("Font family")]
        [XmlIgnore]
        [FontPreview(14)]
        public FontFamily FontFamily
        {
            get { return new FontFamily(FontFamilyName); }
            set
            {
                if (value == null)
                {
                    FontFamilyName = "Consolas";
                    return;
                }
                FontFamilyName = value.Source;
            }
        }

        [DefaultValue("Consolas")]
        [XmlElement("FontFamily")]
        [System.ComponentModel.Browsable(false)]
        public string FontFamilyName
        {
            get { return _fontFamilyName; }
            set
            {
                if (value == _fontFamilyName) return;
                _fontFamilyName = value;
                OnPropertyChanged("FontFamily");
            }
        }

        [System.ComponentModel.DisplayName("Font size")]
        [DefaultValue(12)]
        [ItemsSourceProperty("FontSizes")]
        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                if (value == _fontSize) return;
                _fontSize = value;
                OnPropertyChanged("FontSize");
            }
        }

        [System.ComponentModel.DisplayName("Show line numbers")]
        [DefaultValue(true)]
        public bool ShowLineNumbers
        {
            get { return _showLineNumbers; }
            set
            {
                if (value == _showLineNumbers) return;
                _showLineNumbers = value;
                OnPropertyChanged("ShowLineNumbers");
            }
        }

        [DefaultValue(true)]
        [System.ComponentModel.DisplayName("Highlight current line")]
        public override bool HighlightCurrentLine
        {
            get { return base.HighlightCurrentLine; }
            set { base.HighlightCurrentLine = value; }
        }

        [DefaultValue(true)]
        [System.ComponentModel.DisplayName("Convert tabs to spaces")]
        public override bool ConvertTabsToSpaces
        {
            get { return base.ConvertTabsToSpaces; }
            set { base.ConvertTabsToSpaces = value; }
        }

        [System.ComponentModel.DisplayName("Show spaces")]
        public override bool ShowSpaces
        {
            get { return base.ShowSpaces; }
            set { base.ShowSpaces = value; }
        }

        [System.ComponentModel.DisplayName("Show tabs")]
        public override bool ShowTabs
        {
            get { return base.ShowTabs; }
            set { base.ShowTabs = value; }
        }

        [System.ComponentModel.DisplayName("Show end of line")]
        public override bool ShowEndOfLine
        {
            get { return base.ShowEndOfLine; }
            set { base.ShowEndOfLine = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public override bool ShowBoxForControlCharacters
        {
            get { return base.ShowBoxForControlCharacters; }
            set { base.ShowBoxForControlCharacters = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public override bool EnableHyperlinks
        {
            get { return base.EnableHyperlinks; }
            set { base.EnableHyperlinks = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public override bool EnableEmailHyperlinks
        {
            get { return base.EnableEmailHyperlinks; }
            set { base.EnableEmailHyperlinks = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public override bool RequireControlModifierForHyperlinkClick
        {
            get { return base.RequireControlModifierForHyperlinkClick; }
            set { base.RequireControlModifierForHyperlinkClick = value; }
        }

        [System.ComponentModel.DisplayName("Indentation size")]
        [Spinnable(1, 4, 1, 1000), Width(60)]
        public override int IndentationSize
        {
            get { return base.IndentationSize; }
            set { base.IndentationSize = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public override bool CutCopyWholeLine
        {
            get { return base.CutCopyWholeLine; }
            set { base.CutCopyWholeLine = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public override bool AllowScrollBelowDocument
        {
            get { return base.AllowScrollBelowDocument; }
            set { base.AllowScrollBelowDocument = value; }
        }

        [System.ComponentModel.DisplayName("Word wrap indentation")]
        [Spinnable(1, 4, 0, 256), Width(60)]
        public override double WordWrapIndentation
        {
            get { return base.WordWrapIndentation; }
            set { base.WordWrapIndentation = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public override bool InheritWordWrapIndentation
        {
            get { return base.InheritWordWrapIndentation; }
            set { base.InheritWordWrapIndentation = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public override bool EnableVirtualSpace
        {
            get { return base.EnableVirtualSpace; }
            set { base.EnableVirtualSpace = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public override bool EnableImeSupport
        {
            get { return base.EnableImeSupport; }
            set { base.EnableImeSupport = value; }
        }

        [System.ComponentModel.DisplayName("Show column ruler")]
        public override bool ShowColumnRuler
        {
            get { return base.ShowColumnRuler; }
            set { base.ShowColumnRuler = value; }
        }
        
        [System.ComponentModel.DisplayName("Column ruler position")]
        [DefaultValue(80), Width(60)]
        public override int ColumnRulerPosition
        {
            get { return base.ColumnRulerPosition; }
            set { base.ColumnRulerPosition = value; }
        }

        [System.ComponentModel.Browsable(false)]
        [DefaultValue(true)]
        public new bool EnableRectangularSelection
        {
            get { return base.EnableRectangularSelection; }
            set { base.EnableRectangularSelection = value; }
        }

        [System.ComponentModel.Browsable(false)]
        [DefaultValue(true)]
        public new bool EnableTextDragDrop
        {
            get { return base.EnableTextDragDrop; }
            set { base.EnableTextDragDrop = value; }
        }

        [System.ComponentModel.DisplayName("Hide cursor while typing")]
        [DefaultValue(true)]
        public new bool HideCursorWhileTyping
        {
            get { return base.HideCursorWhileTyping; }
            set { base.HideCursorWhileTyping = value; }
        }

        [System.ComponentModel.Browsable(false)]
        [DefaultValue(false)]
        public new bool AllowToggleOverstrikeMode
        {
            get { return base.AllowToggleOverstrikeMode; }
            set { base.AllowToggleOverstrikeMode = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public List<double> FontSizes
        {
            get
            {
                if (_fontSizes == null)
                {
                    _fontSizes = new List<double>
                    {
                        5.0,
                        5.5,
                        6.0,
                        6.5,
                        7.0,
                        7.5,
                        8.0,
                        8.5,
                        9.0,
                        9.5,
                        10.0,
                        12.0,
                        14.0,
                        16.0,
                        18.0,
                        20.0,
                        24.0
                    };
                }
                return _fontSizes;
            }
        }

        public ExtendedTextEditorOptions CopyFrom(ExtendedTextEditorOptions options)
        {
            var fields =
                typeof (ExtendedTextEditorOptions).GetProperties(BindingFlags.Instance | BindingFlags.Public |
                                                                 BindingFlags.SetProperty);
            foreach (var field in fields)
            {
                var value = field.GetValue(options);
                if (field.CanWrite)
                    field.SetValue(this, value);
            }
            return options;
        }

        public void SetBindings([NotNull] TextEditor textEditor)
        {
            if (textEditor == null) throw new ArgumentNullException("textEditor");

            textEditor.SetBinding(Control.FontFamilyProperty,
                new Binding("FontFamily") {Source = this, Mode = BindingMode.TwoWay});
            textEditor.SetBinding(Control.FontSizeProperty,
                new Binding("FontSize") {Source = this, Mode = BindingMode.TwoWay});
            textEditor.SetBinding(TextEditor.ShowLineNumbersProperty,
                new Binding("ShowLineNumbers") {Source = this, Mode = BindingMode.TwoWay});
            textEditor.SetBinding(TextEditor.OptionsProperty, new Binding {Source = this});
        }
    }
}