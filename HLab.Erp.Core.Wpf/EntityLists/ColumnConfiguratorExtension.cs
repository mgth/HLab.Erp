using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Data;
using HLab.Icons.Wpf.Icons;
using HLab.Localization.Wpf.Lang;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.EntityLists
{
    public static class ColumnConfiguratorExtension
    {
        public static IColumnConfigurator<T, TLink, TFilter> Icon<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Func<T, string> getPath, double size = 30.0)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            var getContent = c.Column.Getter;
            if (getContent == null)
                return c.Content(t => new IconView { Path = getPath(t), IconMaxHeight = size, IconMaxWidth = size });

            return c.Content(t => new IconView { Path = getPath(t) ?? "", IconMaxHeight = size, IconMaxWidth = size, Caption = getContent(t) ?? "" });
        }

        public static IColumnConfigurator<T,TLink,TFilter> Content<T, TLink, TFilter>(this IColumnConfigurator<T,TLink,TFilter> c, Func<T, object> getter)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            c.Column.Getter = getter;
            if (c.Column.OrderBy == null)
            {
                c.Column.OrderBy = getter;
            }
            return c;
        }

        public static IColumnConfigurator<T,TLink,TFilter> Content<T, TLink, TFilter>(this IColumnConfigurator<T,TLink,TFilter> c, Func<T, Task<object>> getter)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            c.Column.Getter = t => new AsyncView { Getter = async () => await getter(t) };
            return c;
        }

        public static IColumnConfigurator<T,TLink,TFilter> Localize<T, TLink, TFilter>(this IColumnConfigurator<T,TLink,TFilter> c)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            var getContent = c.Column.Getter;
            if (getContent != null)
                return c.Content(e => new Localize { Id = (string)getContent(e) });
            else if (c.Column.Header is string header)
            {
                return c.Header(new Localize { Id = header });
            }
            throw new NullReferenceException("Localize must be used on existing content");
        }

        public static IColumnConfigurator<T, TLink, TFilter> Center<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            var getContent = c.Column.Getter;

            return c.Content(t => new ContentControl { VerticalAlignment = VerticalAlignment.Stretch, Content = getContent?.Invoke(t) });
        }

        public static IColumnConfigurator<T, TLink, TFilter> Mvvm<T, TLink, TFilter,TViewClass>(this IColumnConfigurator<T, TLink, TFilter> c, TViewClass viewClass) 
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
            where TViewClass : IViewClass
        {
            var getter = c.Column.Getter;
            c.Column.Getter = o => new ViewLocator { ViewClass = typeof(TViewClass), DataContext = getter(o) };
            return c;
        }
        public static IColumnConfigurator<T, TLink, TFilter> Mvvm<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c) 
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            
            return c.Mvvm(default(IViewClassListItem));
        }

    }
}