using System;
using System.Linq.Expressions;
using HLab.Base.Fluent;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;

namespace HLab.Erp.Core.ListFilters
{
    public static class TextFilterExtension
    {
        public static T Title<T>(this T c, string title)
            where T : IFluentConfigurator<TextFilter>
            => c.Set<T,TextFilter>(f => f.Title = title);

        public static T Value<T>(this T c, string value)
            where T : IFluentConfigurator<TextFilter>
            => c.Set<T,TextFilter>(f => f.Value = value);

        public static TConf Link<TConf,T>(this TConf c,IEntityListViewModel<T> vm, Expression<Func<T, string>> getter)
            where TConf : IFiltersFluentConfigurator<T>, IFluentConfigurator<TextFilter>
            where T : class, IEntity
            => c.Set<TConf,TextFilter>(f =>
            {
                c.List.AddFilter(f.Title,()=> f.Match(getter));
                f.Update = ()=> c.List.UpdateAsync();
            });
    }
}