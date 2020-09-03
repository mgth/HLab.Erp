using System;
using System.Linq.Expressions;
using HLab.Base.Fluent;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;

namespace HLab.Erp.Core.ListFilters
{
    public static class FilterTextExtension
    {
        public static T Title<T>(this T c, string title)
            where T : IFluentConfigurator<FilterTextViewModel>
            => c.Set<T,FilterTextViewModel>(f => f.Title = title);

        public static T Value<T>(this T c, string value)
            where T : IFluentConfigurator<FilterTextViewModel>
            => c.Set<T,FilterTextViewModel>(f => f.Value = value);

        public static TConf Link<TConf,T>(this TConf c,IEntityListViewModel<T> vm, Expression<Func<T, string>> getter)
            where TConf : IFiltersFluentConfigurator<T>, IFluentConfigurator<FilterTextViewModel>
            where T : class, IEntity
            => c.Set<TConf,FilterTextViewModel>(f =>
            {
                c.List.AddFilter(f.Title,()=> f.Match(getter));
                f.Update = ()=> c.List.UpdateAsync();
            });
    }
}