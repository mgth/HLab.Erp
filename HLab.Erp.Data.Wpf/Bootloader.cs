using System.Threading.Tasks;
using System.Windows;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Data.Wpf;

public class ErpDataBootloader(IDataService data, IMvvmService mvvm) : IBootloader
{
    public Task LoadAsync(IBootContext bootstrapper)
    {
        if (bootstrapper.WaitingForService(mvvm)) return Task.CompletedTask;

        data.SetConfigureAction(async () =>
        {

            var connectionData = new ConnectionData();

            var view = await mvvm.MainContext.GetViewAsync(connectionData, typeof(DefaultViewMode), typeof(IDefaultViewClass));

            var dialog = new Window
            {
                Content = view,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.WidthAndHeight
            };

            if (!(dialog.ShowDialog() ?? false)) return "";

            return $"Host={connectionData.Server};Username={connectionData.UserName};Password={connectionData.Password};Database={connectionData.Database}";;
        });

        return Task.CompletedTask;
    }

}