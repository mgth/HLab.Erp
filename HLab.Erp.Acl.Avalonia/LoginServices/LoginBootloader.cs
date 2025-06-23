using Avalonia.Controls;
using Avalonia.Threading;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Avalonia;

namespace HLab.Erp.Acl.Avalonia.LoginServices
{
   public class LoginBootloader(IMvvmService mvvm, Func<ILoginViewModel> getViewModel, IAclService acl)
       : Bootloader
   {
      protected override BootState Load()
      {
         //if we can have localization and picture lets do it
         if (WaitingForBootloader("LocalizeBootloader", "IconBootloader")) return BootState.Requeue;

         Dispatcher.UIThread.Post(async () =>
         {
            //retrieve login window
            var view = await mvvm.MainContext.GetViewAsync(getViewModel(), typeof(DefaultViewMode), default);
            var loginWindow = view.AsWindow();

            //loginWindow.SizeToContent = SizeToContent.WidthAndHeight;
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            loginWindow.ShowDialog(loginWindow); //TODO: check if this is correct

            //if connection failed
            if (acl.Connection is null) Dispatcher.UIThread.BeginInvokeShutdown(DispatcherPriority.Normal); ;
         });

         return base.Load();
      }

   }
}
