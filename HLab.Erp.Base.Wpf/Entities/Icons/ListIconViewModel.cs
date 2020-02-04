using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntityLists;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;

namespace HLab.Erp.Base.Wpf.Entities.Icons
{
    public class ListIconViewModel : EntityListViewModel<ListIconViewModel,Icon>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
        public string Title => "{Icons}";

        private async Task<object> GetSvgIconAsync(string source)
        {
            var icon = (UIElement)await XamlTools.FromSvgStringAsync(source).ConfigureAwait(true);
            return new Viewbox
            {
                Child = icon,
                MaxHeight = 30
            };
        }
        private async Task<object> GetXamlIconAsync(string source)
        {
            var icon = (UIElement)await XamlTools.FromXamlStringAsync(source).ConfigureAwait(true);
            return new Viewbox
            {
                Child = icon,
                MaxHeight = 30
            };
        }

        public ListIconViewModel()
        {
            Columns
                .Column("{Path}", s => s.Path)
                .ColumnAsync("{Xaml}", s => GetXamlIconAsync(s.SourceXaml),null)
                .ColumnAsync("{Svg}", s => GetSvgIconAsync(s.SourceSvg),null)
                ;

            List.UpdateAsync();
        }
    }
}