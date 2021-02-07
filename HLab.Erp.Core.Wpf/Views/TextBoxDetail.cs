using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using HLab.Base;

namespace HLab.Erp.Core.Views
{
    using H = DependencyHelper<TextBoxDetail>;
    public class TextBoxDetail : TextBox
    {
        public string ReactiveText
        {
            get => (string)GetValue(ReactiveTextProperty);
            set => SetValue(ReactiveTextProperty,value);
        }
        public static readonly DependencyProperty ReactiveTextProperty = 
            H.Property<string>()
            .OnChange((e,a) => e.OnReactiveTextChanged(a.NewValue))
            .DefaultUpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged)
            .Register();

        protected virtual void OnReactiveTextChanged(string e)
        {
            Text = e;
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            ReactiveText = Text;
            base.OnTextChanged(e);
        }


    }
}
