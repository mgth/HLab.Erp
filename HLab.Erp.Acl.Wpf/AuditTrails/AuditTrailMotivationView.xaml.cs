using HLab.Mvvm.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HLab.Erp.Acl.AuditTrails;

namespace HLab.Erp.Acl
{
    /// <summary>
    /// Logique d'interaction pour AuditTrailMotivationView.xaml
    /// </summary>
    public partial class AuditTrailMotivationView : UserControl, IView<AuditTrailMotivationViewModel>
    {
        public AuditTrailMotivationView()
        {
            InitializeComponent();
            DataContextChanged += AuditTrailMotivationView_DataContextChanged;
        }

        private void AuditTrailMotivationView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue is AuditTrailMotivationViewModel oldvm) oldvm.PropertyChanged -= Vm_PropertyChanged;
            if(e.NewValue is AuditTrailMotivationViewModel vm) vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName=="Result")
            {
                if(sender is AuditTrailMotivationViewModel vm)
                {
                    if(vm.Result!=null)
                    {
                        var w = Window.GetWindow(this);
                        w.DialogResult = vm.Result;
                        w.Close();
                    }
                }
            }
        }

        private void PasswordBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AuditTrailMotivationViewModel l)
            {
                l.SetPassword(PasswordBox.SecurePassword);
            }

        }
    }
}
