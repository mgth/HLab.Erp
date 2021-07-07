using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Icons.Wpf.Icons;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Icons
{
    public class IconsListViewModel : EntityListViewModel<Icon>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        private static async Task<object> GetSvgIconAsync(string source, int? foreColor)
        {
            if (string.IsNullOrWhiteSpace(source)) return null;
            var icon = (UIElement)await XamlTools.FromSvgStringAsync(source).ConfigureAwait(true);
            return new Viewbox
            {
                Child = icon,
                MaxHeight = 30
            };
        }
        private static async Task<object> GetXamlIconAsync(string source, int? foreColor)
        {
            var icon = (UIElement)await XamlTools.FromXamlStringAsync(source).ConfigureAwait(true);
            return new Viewbox
            {
                Child = icon,
                MaxHeight = 30
            };
        }

        public IconsListViewModel() : base(c => c
// TODO            .AddAllowed()
               .Column()
                   .Header("{Path}")
                   .Link(s => s.Path)
                       .Filter()

               .Column()
                    .Header("{Xaml}")
                    .Content(async s => await GetXamlIconAsync(s.SourceXaml, s.Foreground))

               .Column()
                    .Header("{Svg}")
                    .Content(async s => await GetSvgIconAsync(s.SourceSvg, s.Foreground))
        )
        {
        }
    }
}