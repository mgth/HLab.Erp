using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
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

        private static async Task<object> GetSvgIconAsync(string source, int? foreColor)
        {
            if (string.IsNullOrWhiteSpace(source)) return null;
            var icon = (UIElement)await XamlTools.FromSvgStringAsync(source,foreColor).ConfigureAwait(true);
            return new Viewbox
            {
                Child = icon,
                MaxHeight = 30
            };
        }
        private static async Task<object> GetXamlIconAsync(string source, int? foreColor)
        {
            var icon = (UIElement)await XamlTools.FromXamlStringAsync(source, foreColor).ConfigureAwait(true);
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
                .Column.Header("{Xaml}").Content(async s => await GetXamlIconAsync(s.SourceXaml,s.Foreground))
                .Column.Header("{Svg}").Content(async s => await GetSvgIconAsync(s.SourceSvg,s.Foreground))
            );

            using (List.Suspender.Get())
            {
                Filter<TextFilter>(). Title("{Path}")
                    .Link(List,e => e.Path);
            }
        }
    }
}