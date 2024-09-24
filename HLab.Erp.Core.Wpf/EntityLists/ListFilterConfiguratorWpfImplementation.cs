using HLab.Base;
using HLab.Base.Wpf.Controls;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Icons.Wpf.Icons;
using HLab.Localization.Wpf.Lang;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Wpf;
using HLab.Erp.Core.Wpf.Views;

namespace HLab.Erp.Core.Wpf.EntityLists;

public class ListFilterConfiguratorWpfImplementation : IListFilterConfiguratorPlatformImplementation
{
    public static void Initialize()
    {
        ListFilterConfiguratorExtension.Implementation = new ListFilterConfiguratorWpfImplementation();
    }

    public IColumnConfigurator<T, TLink, TFilter> 
        Localize<T, TLink, TFilter>(IColumnConfigurator<T, TLink, TFilter> @this, string content) 
        where T : class, IEntity, new() 
        where TFilter : class, IFilter<TLink>
        => @this.ContentTemplate($$"""<{{XamlTool.Type<Localize>()}} Id="{Binding {{content}}}"/>""");

    public IColumnConfigurator<T,TLink,TFilter> 
        Progress<T, TLink, TFilter>
        (
            IColumnConfigurator<T,TLink,TFilter> @this,
            string progress
        )
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
        => @this.ContentTemplate($$"""<{{XamlTool.Type<ProgressView>()}} Value="{Binding {{progress}}}" VerticalAlignment="Stretch"/>""");

    public IColumnConfigurator<T,TLink,TFilter> 
        Description<T, TLink, TFilter>
        (
            IColumnConfigurator<T,TLink,TFilter> @this,
            string title,
            string description
        )
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
        => @this.ContentTemplate($$"""<{{XamlTool.Type<ColumnDescriptionBlock>()}} VerticalAlignment="Top" Title="{Binding {{title}}}" Description="{Binding {{description}}}"/>""");

    public IColumnConfigurator<T, TLink, TFilter> 
        Date<T, TLink, TFilter>(IColumnConfigurator<T, TLink, TFilter> @this, string content) 
        where T : class, IEntity, new() 
        where TFilter : class, IFilter<TLink>
        => @this.ContentTemplate($$"""<{{XamlTool.Type<DateColumnItem>()}} HorizontalAlignment="Right" Date="{Binding {{content}}}" DayValid="True"/>""");

    public IColumnConfigurator<T, TLink, TFilter> 
        Date<T, TLink, TFilter>(IColumnConfigurator<T, TLink, TFilter> @this, string date, string dayValid) 
        where T : class, IEntity, new() 
        where TFilter : class, IFilter<TLink>
        => @this.ContentTemplate($$"""<{{XamlTool.Type<DateColumnItem>()}} HorizontalAlignment="Right" Date="{Binding {{date}}}" DayValid="{Binding {{dayValid}}}"/>""");

   public IColumnConfigurator<T, TLink, TFilter> 
        Icon<T, TLink, TFilter>(IColumnConfigurator<T, TLink, TFilter> @this, string iconPath, double size) 
        where T : class, IEntity, new() 
        where TFilter : class, IFilter<TLink>
        => @this.DecorateTemplate($$"""<{{XamlTool.Type<IconView>(out var ns)}} Path="{Binding {{iconPath}}}" IconMaxHeight ="{{size}}" IconMaxWidth = "{{size}}">{{XamlTool.ContentPlaceHolder}}</{{XamlTool.Type<IconView>(ns)}}>""");

   public IColumnConfigurator<T, TLink, TFilter> 
        Mvvm<T, TLink, TFilter, TViewClass>(IColumnConfigurator<T, TLink, TFilter> @this, TViewClass viewClass) 
        where T : class, IEntity, new() 
        where TFilter : class, IFilter<TLink> 
        where TViewClass : IViewClass
       => @this.DecorateTemplate($$"""
                                   <{{XamlTool.Type<ViewLocator>(out var ns1)}}
                                        {{XamlTool.Namespace<TViewClass>(out var ns2)}}
                                        ViewClass="{x:Type {{XamlTool.Type<TViewClass>(ns2)}}}">
                                            <{{XamlTool.Type<ViewLocator>(ns1)}}.Model>{{XamlTool.ContentPlaceHolder}}</{{XamlTool.Type<ViewLocator>(ns1)}}.Model>
                                   </{{XamlTool.Type<ViewLocator>(ns1)}}>
                                   """);
}

