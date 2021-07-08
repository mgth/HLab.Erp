using System;
using HLab.Base.Fluent;

namespace HLab.Erp.Core.Wpf.ListFilters
{
    public static class DateFilterExtension
    {
        public static T Header<T>(this T c, object header)
            where T : IFluentConfigurator<DateFilter>
            => c.Set<T, DateFilter>(f => f.Header = header);

        public static T MinDate<T>(this T c, DateTime value)
            where T : IFluentConfigurator<DateFilter>
            => c.Set<T, DateFilter>(f => f.MinDate = value);
        public static T MaxDate<T>(this T c, DateTime value)
            where T : IFluentConfigurator<DateFilter>
            => c.Set<T, DateFilter>(f => f.MaxDate = value);
    }
}