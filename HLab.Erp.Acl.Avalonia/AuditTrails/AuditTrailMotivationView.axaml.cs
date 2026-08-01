using System.Net;
using System.Security;
using HLab.Mvvm.Annotations;
using Avalonia.Controls;
using HLab.Erp.Acl.AuditTrails;

namespace HLab.Erp.Acl.Avalonia.AuditTrails
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

        AuditTrailMotivationViewModel? _oldViewModel = null;
        private void AuditTrailMotivationView_DataContextChanged(object? sender, EventArgs e)
        {
            if (_oldViewModel != null) _oldViewModel.PropertyChanged -= Vm_PropertyChanged;
            if (DataContext is AuditTrailMotivationViewModel vm)
            {
                vm.PropertyChanged += Vm_PropertyChanged;
                _oldViewModel = vm;
            }
        }

        void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs? e)
        {
            if (e.PropertyName == "Result")
            {
                if (sender is AuditTrailMotivationViewModel vm)
                {
                    if (vm.Result != null)
                    {
                        if (TopLevel.GetTopLevel(this) is Window w)
                        {
                            if (w is HLab.Mvvm.Avalonia.DefaultWindow dw) dw.DialogResult = vm.Result;
                            w.Close(vm.Result);
                        }
                    }
                }
            }
        }

        void PasswordBox_OnTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (DataContext is AuditTrailMotivationViewModel l)
            {
                l.SetPassword(new NetworkCredential("", PasswordBox.Text).SecurePassword);
            }
        }
    }
}
