using System.Threading.Tasks;
using System.Windows;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Data.Wpf;

public class ErpDataBootloader(IDataService data, IMvvmService mvvm) : IBootloader
{
    public async Task LoadAsync(IBootContext bootstrapper)
    {
        //if (_mvvm.ServiceState != ServiceState.Available)
        //{
        //    bootstrapper.Requeue();
        //    return;
        //}


        data.SetConfigureAction(async () =>
        {

            var data = new ConnectionData();

            var view = await mvvm.MainContext.GetViewAsync(data, typeof(DefaultViewMode), typeof(IDefaultViewClass));

            var dialog = new Window
            {
                Content = view,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.WidthAndHeight
            };

            if (dialog.ShowDialog() ?? false)
            {
                return $"Host={data.Server};Username={data.UserName};Password={data.Password};Database={data.Database}";;

            }

            return "";
        });
    }

}