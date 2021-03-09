using System;
using System.Linq.Expressions;
using HLab.Base.Fluent;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;

namespace HLab.Erp.Core.ListFilters
{
    public static class FilterDateExtension
    {
        public static T Title<T>(this T c, string title)
            where T : IFluentConfigurator<DateFilter>
            => c.Set<T,DateFilter>(f => f.Title = title);

        public static T MinDate<T>(this T c, DateTime value)
            where T : IFluentConfigurator<DateFilter>
            => c.Set<T,DateFilter>(f => f.MinDate = value);
        public static T MaxDate<T>(this T c, DateTime value)
            where T : IFluentConfigurator<DateFilter>
            => c.Set<T,DateFilter>(f => f.MaxDate = value);

        public static TConf Link<TConf,T>(this TConf c,IEntityListViewModel<T> vm, Expression<Func<T, DateTime?>> getter)
            where TConf : IFiltersFluentConfigurator<T>, IFluentConfigurator<DateFilter>
            where T : class, IEntity
            => c.Set<TConf,DateFilter>(f =>
            {
                c.List.AddFilter(f.Title,()=> f.Match(getter));
                f.Update = ()=> c.List.UpdateAsync();
            });
    }
}