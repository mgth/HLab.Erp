using System;
using System.Windows;
using HLab.Core.Annotations;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Views;
using HLab.Mvvm.Wpf.Views;

namespace HLab.Erp.Acl.LoginServices
{
    public class LoginBootloader : IBootloader
    {
        readonly Func<ILoginViewModel> _getViewModel;
        readonly IMvvmService _mvvm;
        readonly IAclService _acl;

        public LoginBootloader(IMvvmService mvvm, Func<ILoginViewModel> getViewModel, IAclService acl)
        {
            _mvvm = mvvm;
            _acl = acl;

            _getViewModel = getViewModel;
        }

        public void Load(IBootContext bootstrapper)
        {
            //if we can have localization and picture lets do it
            if (bootstrapper.WaitDependency("LocalizeBootloader", "IconBootloader")) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                //retrieve login window
                var loginWindow = _mvvm.MainContext.GetView(_getViewModel(),typeof(ViewModeDefault)).AsWindow();
                //loginWindow.SizeToContent = SizeToContent.WidthAndHeight;
                loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                loginWindow.ShowDialog();

                //if connection failed
                if (_acl.Connection is null) Application.Current.Shutdown();
            });
        }
    }
}
