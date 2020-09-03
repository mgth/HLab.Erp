using System;
using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Core;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Base.Wpf.Entities.Icons
{
    /// <summary>
    /// Logique d'interaction pour IconView.xaml
    /// </summary>
    public partial class IconEditorView : UserControl, IView<IconViewModel>, IViewClassDocument
    {
        public IconEditorView()
        {
            InitializeComponent();
            DataContextChanged += TestClassView_DataContextChanged;
        }

        private void TestClassView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue is IconViewModel oldVm)
                oldVm.Model.PropertyChanged -= Vm_PropertyChanged;

            if (e.NewValue is IconViewModel vm)
            {
                XamlEditor.Text = vm.Model.SourceXaml;
                SvgEditor.Text = vm.Model.SourceSvg;
                vm.Model.PropertyChanged += Vm_PropertyChanged;
            }
        }

        private bool _changingXaml = false;
        private bool _changingSvg = false;

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            

            switch (e.PropertyName)
            {
                case "SourceXaml":
                    if (_changingXaml) return;
                    XamlEditor.Text = ((IconViewModel)DataContext).Model.SourceXaml;
                    break;
                case "SourceSvg":
                    if (_changingSvg) return;
                    SvgEditor.Text = ((IconViewModel)DataContext).Model.SourceSvg;
                    break;
            }
        }

        private void TextEditor_OnTextChanged(object sender, EventArgs e)
        {
            if (ReferenceEquals(sender, XamlEditor))
            {
                _changingXaml = true;
                ((IconViewModel) DataContext).Model.SourceXaml = XamlEditor?.Text;
                _changingXaml = false;
            }

            if (ReferenceEquals(sender, SvgEditor))
            {
                _changingSvg = true;
                ((IconViewModel) DataContext).Model.SourceSvg = SvgEditor?.Text;
                _changingSvg = false;
            }
        }
    }
}
