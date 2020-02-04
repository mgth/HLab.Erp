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

        public async void Load()
        {
            var icons =  _db.FetchAsync<Icon>().ConfigureAwait(true);

            await foreach (var icon in icons)
            {
                switch (icon.SourceXaml)
                {
                    case "svg":
                        _icons.AddIconProvider(icon.Path, new IconProviderSvgFromSource(icon.SourceSvg, icon.Path));                
                        break;
                    case "xaml":
                        _icons.AddIconProvider(icon.Path, new IconProviderXamlFromSource(icon.SourceSvg, icon.Path));                
                        break;
                }
            }
        }
    }

}
