using System;
using System.Windows;
using Grace.DependencyInjection.Attributes;
using HLab.Core.Annotations;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Views;

namespace HLab.Erp.Acl.LoginServices
{
    public class LoginBootloader : IBootloader
    {
        private readonly Func<ILoginViewModel> _getViewModel;
        private readonly IMvvmService _mvvm;
        private readonly IAclService _acl;

        public LoginBootloader(IMvvmService mvvm, Func<ILoginViewModel> getViewModel, IAclService acl)
        {
            _mvvm = mvvm;
            _acl = acl;

            _getViewModel = getViewModel;
        }

        public void Load(IBootContext bootstrapper)
        {
            var loginWindow = _mvvm.MainContext.GetView(_getViewModel(),typeof(ViewModeDefault)).AsWindow();
            loginWindow.SizeToContent = SizeToContent.WidthAndHeight;
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            loginWindow.ShowDialog();

            if (_acl.Connection is not null) return;

            Application.Current.Shutdown();
        }
    }
}
