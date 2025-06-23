using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Icons.Wpf.Icons.Providers;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf;

public class DbIconModule(IIconService icons, IDataService data) : Bootloader
{
    public override async Task<BootState> LoadAsync()
    {
        if (data.ServiceState != ServiceState.Available) return BootState.Requeue;
        var dataIcons =  data.FetchAsync<Icon>().ConfigureAwait(true);

        try
        {
            await foreach (var icon in dataIcons)
            {
                var path = icon.Path.ToLower();

                if (!string.IsNullOrWhiteSpace(icon.SourceXaml))
                {
                    icons.AddIconProvider(path, new IconProviderXamlFromSource(icon.SourceXaml, path, icon.Foreground));
                }
                else if (!string.IsNullOrWhiteSpace(icon.SourceSvg))
                {
                    icons.AddIconProvider(path, new IconProviderSvgFromSource(icon.SourceSvg, path, icon.Foreground));
                }
            }
        }
        catch (DataException)
        {

        }
      
        return BootState.Completed;

    }

}