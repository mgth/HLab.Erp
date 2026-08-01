using System;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using HLab.Core.Annotations;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Options;
using HLab.UI;

namespace HLab.Erp.Acl.LoginServices;

public class LoginBootloader(
   IMvvmService mvvm,
   Func<ILoginViewModel> getViewModel,
   IAclService acl,
   IIconService icons,
   ILocalizationService localize,
   IDataService data,
   IOptionsService options
   ) : Bootloader
{
   public override async Task<BootState> LoadAsync()
   {
      //if we can have localization and picture lets do it
      //mvvm : views must be registered before the login view can resolve
      //options : le préremplissage DEBUG lit le provider registre, qui doit être enregistré
      if (WaitingForServices(localize, icons, data, mvvm, options)) return BootState.Requeue;

      await UiPlatform.InvokeOnUiThreadAsync(async () =>
      {
          var viewmodel = getViewModel();

          // Défaut : compte Windows, sans écraser un éventuel préremplissage (DebugUsername)
          if (string.IsNullOrEmpty(viewmodel.Username))
              viewmodel.Username = Environment.UserName;
          //retrieve login window
          var view = mvvm.MainContext.GetView(viewmodel, typeof(DefaultViewMode), typeof(IDefaultViewClass));
          var loginWindow = mvvm.ViewAsWindow(view);
          //loginWindow.SizeToContent = SizeToContent.WidthAndHeight;
          //loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
          loginWindow.ShowDialog();

          //if connection failed
      });

      if (acl.Connection is null) UiPlatform.Quit();
      
      return await base.LoadAsync();
   }
}