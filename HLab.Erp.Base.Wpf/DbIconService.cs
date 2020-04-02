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
        private IconService _icons;
        [Import]
        private IDataService _db;

        public bool Load()
        {
            LoadAsync();
            return true;
        }

        public async void LoadAsync()
        {
            var icons =  _db.FetchAsync<Icon>().ConfigureAwait(true);

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
