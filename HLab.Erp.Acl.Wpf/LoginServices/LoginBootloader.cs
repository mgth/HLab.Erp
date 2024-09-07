using System;
using System.Threading.Tasks;
using System.Windows;
using HLab.Core.Annotations;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Wpf.Views;

namespace HLab.Erp.Acl.LoginServices;

public class LoginBootloader(IMvvmService mvvm, Func<ILoginViewModel> getViewModel, IAclService acl)
    : IBootloader
{
    public async Task LoadAsync(IBootContext bootstrapper)
    {
        //if we can have localization and picture lets do it
        if (bootstrapper.WaitDependency("LocalizeBootloader", "IconBootloader")) return;

        await Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            //retrieve login window
            var view = await mvvm.MainContext.GetViewAsync(getViewModel(),typeof(DefaultViewMode));
            var loginWindow = view.AsWindow();
            //loginWindow.SizeToContent = SizeToContent.WidthAndHeight;
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            loginWindow.ShowDialog();

            //if connection failed
            if (acl.Connection is null) Application.Current.Shutdown();
        });
    }
}