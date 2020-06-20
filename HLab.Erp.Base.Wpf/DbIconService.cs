using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;

namespace HLab.Erp.Base.Wpf
{
    public class DbIconModule : IBootloader
    {
        [Import]
        private IIconService _icons;
        [Import]
        private IDataService _data;

        public void Load(IBootContext b)
        {
            if (_data.ServiceState != ServiceState.Available)
            {
                b.Requeue(); return;
            }

            LoadAsync();
        }

        public async void LoadAsync()
        {
            var icons =  _data.FetchAsync<Icon>().ConfigureAwait(true);

            try
            {
                await foreach (var icon in icons)
                {
                    if (!string.IsNullOrWhiteSpace(icon.SourceXaml))
                    {
                        _icons.AddIconProvider(icon.Path, new IconProviderSvgFromSource(icon.SourceSvg, icon.Path));
                    }
                    else if (!string.IsNullOrWhiteSpace(icon.SourceSvg))
                    {
                        _icons.AddIconProvider(icon.Path, new IconProviderXamlFromSource(icon.SourceSvg, icon.Path));
                    }
                }
            }
            catch (DataException)
            {

            }
        }
    }

}
