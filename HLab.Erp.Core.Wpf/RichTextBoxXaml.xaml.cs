using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace HLab.Erp.Core
{
    /// <summary>
    /// Logique d'interaction pour RichTextBoxXaml.xaml
    /// </summary>
    public partial class RichTextBoxXaml : UserControl
    {
        public RichTextBoxXaml()
        {
            InitializeComponent();
            RichTextBox.TextChanged += RichTextBoxOnTextChanged;
        }



        private string _xaml = null;

        private void RichTextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            _xaml = XamlWriter.Save(RichTextBox.Document);
            DocumentXaml = _xaml;
        }


        public static readonly DependencyProperty DocumentXamlProperty =
            DependencyProperty.Register(nameof(DocumentXaml), typeof(string), typeof(RichTextBoxXaml), new PropertyMetadata(OnDocumentXamlChanged));
        public string DocumentXaml
        {
            get => (string)GetValue(DocumentXamlProperty); set => SetValue(DocumentXamlProperty, value);
        }
        private static void OnDocumentXamlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RichTextBoxXaml)?.SetDocumentFromXaml();
        }

        public void SetDocumentFromXaml()
        {
            if (DocumentXaml != _xaml)
            {
                try
                {
                    var stream = new MemoryStream(Encoding.UTF8.GetBytes(DocumentXaml));
                    var doc = (FlowDocument)XamlReader.Load(stream);

                    // Set the document
                    RichTextBox.Document = doc;
                }
                catch (Exception)
                {
                    RichTextBox.Document = new FlowDocument();
                }
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var selection = RichTextBox.Selection;
            if (!selection.IsEmpty)
                selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);

            var anchor = new Span(selection.Start,selection.End);

            anchor.Name = "aaa";
        }

    }
}
