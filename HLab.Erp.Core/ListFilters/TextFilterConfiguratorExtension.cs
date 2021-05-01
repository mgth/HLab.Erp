using System;
using System.Linq.Expressions;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ListFilters
{
    public static class TextFilterConfiguratorExtension
    {

        public static IFilterConfigurator<T, TextFilter> PostLink<T>(this IFilterConfigurator<T, TextFilter> tf, Func<T, string> getter) where T : class, IEntity, new()
        {
            tf.CurrentFilter.PostLink<T>(tf.Target().List, getter);
            return tf;
        }
        public static IFilterConfigurator<T, TextFilter> Link<T>(this IFilterConfigurator<T, TextFilter> tf, Expression<Func<T, string>> getter) where T : class, IEntity, new()
        {
            tf.CurrentFilter.Link<T>(tf.Target().List, getter);
            return tf;
        }
    }
}