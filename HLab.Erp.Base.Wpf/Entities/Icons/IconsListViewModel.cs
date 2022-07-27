using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Data;
using HLab.Icons.Wpf.Icons;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Icons
{
    public class IconsListViewModel : Core.EntityLists.EntityListViewModel<Icon>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";
        }

        protected override bool CanExecuteExport(Action<string> errorAction) => true;

        protected override bool CanExecuteImport(Action<string> errorAction) => true;

        protected override async Task ImportAsync(IDataService data, Icon newValue)
        {
            var icon = await data.FetchOneAsync<Icon>(i => i.Path == newValue.Path);
            if(icon != null)
            {
                if(newValue.Foreground.HasValue)
                    icon.Foreground = newValue.Foreground;

                if(newValue.SourceXaml != null)
                    icon.SourceXaml = newValue.SourceXaml;

                if(newValue.SourceSvg != null)
                    icon.SourceSvg = newValue.SourceSvg;


                await data.UpdateAsync(icon, "Foreground", "SourceXaml", "SourceSvg");
            }
            else
            {
                await data.AddAsync<Icon>(i =>
                {
                    i.Path = newValue.Path;
                    i.Foreground = newValue.Foreground;
                    i.SourceXaml = newValue.SourceXaml;
                    i.SourceSvg = newValue.SourceSvg;
                });

            }
        }

        protected override bool CanExecuteDelete(Icon arg, Action<string> errorAction) => true;
        protected override bool CanExecuteAdd(Action<string> errorAction) => true;

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        static async Task<object> GetSvgIconAsync(string source, int? foreColor)
        {
            if (string.IsNullOrWhiteSpace(source)) return null;
            var icon = (UIElement)await XamlTools.FromSvgStringAsync(source).ConfigureAwait(true);
            return new Viewbox
            {
                Child = icon,
                MaxHeight = 30
            };
        }

        static async Task<object> GetXamlIconAsync(string source, int? foreColor)
        {
            var icon = (UIElement)await XamlTools.FromXamlStringAsync(source).ConfigureAwait(true);
            return new Viewbox
            {
                Child = icon,
                MaxHeight = 30
            };
        }

        public IconsListViewModel(Injector i) : base(i, c => c
               .Column("Path")
                   .Header("{Path}")
                   .Link(s => s.Path)
                       .Filter()

               .Column("Xaml")
                    .Header("{Xaml}")
                    .Content(async s => await GetXamlIconAsync(s.SourceXaml, s.Foreground))

               .Column("Svg")
                    .Header("{Svg}")
                    .Content(async s => await GetSvgIconAsync(s.SourceSvg, s.Foreground))
        )
        {
        }
    }
}