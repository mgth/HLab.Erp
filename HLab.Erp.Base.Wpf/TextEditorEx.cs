using System;
using System.Windows;
using HLab.Base.Wpf;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Search;

namespace HLab.Erp.Base.Wpf
{
    using H = DependencyHelper<TextEditorEx>;
    
    public class TextEditorEx : TextEditor

    {
        public TextEditorEx()
        {
            SearchPanel.Install(this);
        }

        public static readonly DependencyProperty TextProperty = H.Property<string>()
            .OnChange((e,a) => e.OnExTextChanged(a))
            .BindsTwoWayByDefault
            .Register();

        private bool _internalChange = false;

        private void OnExTextChanged(DependencyPropertyChangedEventArgs<string> args)
        {
            if (_internalChange) return;

            base.Text = args.NewValue;
            var document = Document;
            if(Document!=null)
                document.Text = args.NewValue ?? string.Empty;
            //this.CaretOffset = 0;
            //document.UndoStack.ClearAll();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            _internalChange = true;
            Text = base.Text; 
            _internalChange = false;
            base.OnTextChanged(e);
        }

        public new string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}