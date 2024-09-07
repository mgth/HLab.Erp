using HLab.Mvvm.Annotations;
using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Acl.AuditTrails;

namespace HLab.Erp.Acl;

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

    void AuditTrailMotivationView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if(e.OldValue is AuditTrailMotivationViewModel oldvm) oldvm.PropertyChanged -= Vm_PropertyChanged;
        if(e.NewValue is AuditTrailMotivationViewModel vm) vm.PropertyChanged += Vm_PropertyChanged;
    }

    void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

    void PasswordBox_OnTextChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuditTrailMotivationViewModel l)
        {
            l.SetPassword(PasswordBox.SecurePassword);
        }

    }
}