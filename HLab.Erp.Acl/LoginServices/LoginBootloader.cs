using System;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using HLab.Core.Annotations;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.UI;

namespace HLab.Erp.Acl.LoginServices;

public class LoginBootloader(
   IMvvmService mvvm,
   Func<ILoginViewModel> getViewModel,
   IAclService acl,
   IIconService icons,
   ILocalizationService localize,
   IDataService data
   ) : Bootloader
{
   public override async Task<BootState> LoadAsync()
   {
      //if we can have localization and picture lets do it
      if (WaitingForServices(localize, icons, data)) return BootState.Requeue;

      //await UiPlatform.InvokeOnUiThreadAsync(async () =>
      //{
      var viewmodel = getViewModel();
      //retrieve login window
      var view = await mvvm.MainContext.GetViewAsync(viewmodel, typeof(DefaultViewMode));
      var loginWindow = mvvm.ViewAsWindow(view);
      //loginWindow.SizeToContent = SizeToContent.WidthAndHeight;
      //loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
      loginWindow.ShowDialog();

      //if connection failed
      if (acl.Connection is null) UiPlatform.Quit();
      //});
      
      return await base.LoadAsync();
   }
}