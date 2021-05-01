using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Data;
using HLab.Icons.Wpf.Icons;
using HLab.Localization.Wpf.Lang;
using HLab.Mvvm;

namespace HLab.Erp.Core.EntityLists
{
    public static class ColumnConfiguratorExtension
    {
        public static IColumnConfigurator<T> Icon<T>(this IColumnConfigurator<T> c, Func<T, string> getPath, double size = 30.0)
            where T : class, IEntity, new()
        {
            var getContent = c.CurrentColumn.Getter;
            if (getContent == null)
                return c.Content(t => new IconView { Path = getPath(t), IconMaxHeight = size, IconMaxWidth = size });

            return c.Content(t => new IconView { Path = getPath(t) ?? "", IconMaxHeight = size, IconMaxWidth = size, Caption = getContent(t) ?? "" });
        }
        public static IColumnConfigurator<T> Content<T>(this IColumnConfigurator<T> c, Func<T, object> getter)
            where T : class, IEntity, new()
        {
            c.CurrentColumn.Getter = getter;
            if (c.CurrentColumn.OrderBy == null)
            {
                c.CurrentColumn.OrderBy = getter;
            }
            return c;
        }
        public static IColumnConfigurator<T> Content<T>(this IColumnConfigurator<T> c, Func<T, Task<object>> getter)
            where T : class, IEntity, new()
        {
            c.CurrentColumn.Getter = t => new AsyncView { Getter = async () => await getter(t) };
            return c;
        }

        public static IColumnConfigurator<T> Localize<T>(this IColumnConfigurator<T> c)
            where T : class, IEntity, new()
        {
            var getContent = c.CurrentColumn.Getter;
            if (getContent != null)
                return c.Content(e => new Localize { Id = (string)getContent(e) });
            else if (c.CurrentColumn.Header is string header)
            {
                return c.Header(new Localize { Id = header });
            }
            throw new NullReferenceException("Localize must be used on existing content");
        }

        public static IColumnConfigurator<T> Center<T>(this IColumnConfigurator<T> c)
            where T : class, IEntity, new()
        {
            var getContent = c.CurrentColumn.Getter;

            return c.Content(t => new ContentControl { VerticalAlignment = VerticalAlignment.Stretch, Content = getContent?.Invoke(t) });
        }


    }
}