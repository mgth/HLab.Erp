using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntityLists;
using HLab.Icons.Wpf;
using HLab.Icons.Wpf.Icons;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Icons
{
    public class IconsListViewModel : EntityListViewModel<Icon>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        private static async Task<object> GetSvgIconAsync(string source)
        {
            if (string.IsNullOrWhiteSpace(source)) return null;
            var icon = (UIElement)await XamlTools.FromSvgStringAsync(source).ConfigureAwait(true);
            return new Viewbox
            {
                Child = icon,
                MaxHeight = 30
            };
        }
        private static async Task<object> GetXamlIconAsync(string source)
        {
            var icon = (UIElement)await XamlTools.FromXamlStringAsync(source).ConfigureAwait(true);
            return new Viewbox
            {
                Child = icon,
                MaxHeight = 30
            };
        }

        public IconsListViewModel()
        {
            AddAllowed = true;

            Columns.Configure(c => c
                .Column.Header("{Path}").Content(s => s.Path)
                .Column.Header("{Xaml}").Content(async s => await GetXamlIconAsync(s.SourceXaml))
                .Column.Header("{Svg}").Content(async s => await GetSvgIconAsync(s.SourceSvg))
            );

            List.UpdateAsync();
        }
    }
}