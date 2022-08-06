using System;
using System.Windows;
using System.Windows.Controls;
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

        void TestClassView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
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

        bool _changingXaml = false;
        bool _changingSvg = false;

        void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SourceXaml":
                    if (_changingXaml) return;

                    Dispatcher.Invoke(() =>
                    {
                        if (DataContext is IconViewModel vm)
                        {
                            XamlEditor.Text = vm.Model.SourceXaml;
                        }
                    });
                    break;

                case "SourceSvg":
                    if (_changingSvg) return;
                    
                    Dispatcher.Invoke(() =>
                    {
                        if (DataContext is IconViewModel vm)
                        {
                            SvgEditor.Text = vm.Model.SourceSvg;
                        }
                    });
                    
                    break;
            }
        }

        void TextEditor_OnTextChanged(object sender, EventArgs e)
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
