using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Options;

namespace HLab.Erp.Data;

public class ErpDataBootloader(IDataService data, IMvvmService mvvm, IOptionsService options) : IBootloader
{
    public Task LoadAsync(IBootContext bootstrapper)
    {
        if (bootstrapper.WaitingForService(mvvm)) return Task.CompletedTask;
        if (bootstrapper.WaitingForService(options)) return Task.CompletedTask;

        data.SetConfigureAction(async () =>
        {

            var connectionData = new ConnectionData();

            var view = await mvvm.MainContext.GetViewAsync(connectionData, typeof(DefaultViewMode), typeof(IDefaultViewClass));


            var dialog = mvvm.ViewAsWindow(view);
            //{
            //    Content = view,
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
            //    SizeToContent = SizeToContent.WidthAndHeight
            //};

            if (!(dialog.ShowDialog() ?? false)) return "";

            return $"Host={connectionData.Server};Username={connectionData.UserName};Password={connectionData.Password};Database={connectionData.Database}";
        });

        return Task.CompletedTask;
    }

}