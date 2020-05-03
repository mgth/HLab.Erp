using System;
using System.Linq.Expressions;
using HLab.Base.Fluent;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ListFilters
{
    public static class FilterDateExtension
    {
        public static T Title<T>(this T c, string title)
            where T : IFluentConfigurator<FilterDateViewModel>
            => c.Set<T,FilterDateViewModel>(f => f.Title = title);

        public static T MinDate<T>(this T c, DateTime value)
            where T : IFluentConfigurator<FilterDateViewModel>
            => c.Set<T,FilterDateViewModel>(f => f.MinDate = value);
        public static T MaxDate<T>(this T c, DateTime value)
            where T : IFluentConfigurator<FilterDateViewModel>
            => c.Set<T,FilterDateViewModel>(f => f.MaxDate = value);

        public static TConf Link<TConf,T>(this TConf c,IEntityListViewModel<T> vm, Expression<Func<T, DateTime?>> getter)
            where TConf : IFiltersFluentConfigurator<T>, IFluentConfigurator<FilterDateViewModel>
            where T : class, IEntity
            => c.Set<TConf,FilterDateViewModel>(f =>
            {
                c.List.AddFilter(f.Title,()=> f.Match(getter));
                f.Update = ()=> c.List.UpdateAsync();
            });
    }
    public class FilterDateViewModel : FilterViewModel<FilterDateViewModel>
    {

        public Action Update
        {
            get => _update.Get();
            set => _update.Set(value);
        }
        private readonly IProperty<Action> _update = H.Property<Action>();
        
        public DateTime ReferenceDate {
            get => _referenceDate.Get();
            set => _referenceDate.Set(value);
        }
        private readonly IProperty<DateTime> _referenceDate = H.Property<DateTime>();

        /// <summary>
        /// Min Date
        /// </summary>
        public DateTime MinDate
        {
            get => _minDate.Get();
            set
            {
                MinDateCalculated = false;
                _minDate.Set(value);
            }
        }

        private readonly IProperty<DateTime> _minDate = H.Property<DateTime>(c => c
            .On(e => e.MinDateCalculated)
            .On(e => e.MinDateShift)
            .On(e => e.MinDateShiftUnit)
            .When(e => e.MinDateCalculated)
            .Set(e => Shift(e.ReferenceDate, e.MinDateShift, e.MinDateShiftUnit))
        );

        public bool MinDateCalculated {
            get => _minDateCalculated.Get();
            set => _minDateCalculated.Set(value);
        }
        private readonly IProperty<bool> _minDateCalculated = H.Property<bool>(c => c.Default(false));

        public bool MinDateEnabled => _minDateEnabled.Get();
        private readonly IProperty<bool> _minDateEnabled = H.Property<bool>(c => c
            .Default(true)
            .On(e => e.MinDateCalculated)
            .Set(e => !e.MinDateCalculated));

        public int MinDateShift
        {
            get => _minDateShift.Get();
            set => _minDateShift.Set(value);
        }
        private readonly IProperty<int> _minDateShift = H.Property<int>(c => c
            .On(e => e.MinDateCalculated)
            .On(e => e.MinDate)
            .When(e => !e.MinDateCalculated)
            .Do((e, f) =>
                {
                    (e.MinDateShift, e.MinDateShiftUnit) = CalculateShift(e.ReferenceDate, e.MinDate);
                }
                )
        );

        public DateShiftUnit MinDateShiftUnit
        {
            get => _minDateShiftUnit.Get();
            set => _minDateShiftUnit.Set(value);
        }
        private readonly IProperty<DateShiftUnit> _minDateShiftUnit = H.Property<DateShiftUnit>();

        /// <summary>
        /// Max Date
        /// </summary>
        public DateTime MaxDate
        {
            get => _maxDate.Get();
            set
            {
                MaxDateCalculated = false;
                _maxDate.Set(value);
            }
        }

        private readonly IProperty<DateTime> _maxDate = H.Property<DateTime>(c => c
            .On(e => e.MaxDateCalculated)
            .On(e => e.MaxDateShift)
            .On(e => e.MaxDateShiftUnit)
            .When(e => e.MaxDateCalculated)
            .Set(e => Shift(e.ReferenceDate, e.MaxDateShift, e.MaxDateShiftUnit))
        );

        public bool MaxDateCalculated {
            get => _maxDateCalculated.Get();
            set => _maxDateCalculated.Set(value);
        }
        private readonly IProperty<bool> _maxDateCalculated = H.Property<bool>(c => c.Default(false));

        public bool MaxDateEnabled => _maxDateEnabled.Get();
        private readonly IProperty<bool> _maxDateEnabled = H.Property<bool>(c => c
            .Default(true)
            .On(e => e.MaxDateCalculated)
            .Set(e => !e.MaxDateCalculated));

        public int MaxDateShift
        {
            get => _maxDateShift.Get();
            set => _maxDateShift.Set(value);
        }

        private readonly IProperty<int> _maxDateShift = H.Property<int>(c => c
            .On(e => e.MaxDateCalculated)
            .On(e => e.MaxDate)
            .When(e => !e.MaxDateCalculated)
            .Do((e, f) =>
                {
                    (e.MaxDateShift, e.MaxDateShiftUnit) = CalculateShift(e.ReferenceDate, e.MaxDate);
                }
            )
        );
        public DateShiftUnit MaxDateShiftUnit
        {
            get => _maxDateShiftUnit.Get();
            set => _maxDateShiftUnit.Set(value);
        }
        private readonly IProperty<DateShiftUnit> _maxDateShiftUnit = H.Property<DateShiftUnit>();



        public Expression<Func<T,bool>> Match<T>(Expression<Func<T, DateTime?>> getter)
        {
            if (!Enabled) return null;

            var entity = getter.Parameters[0];
            var minDate = Expression.Constant(MinDate, typeof(DateTime?));
            var maxDate = Expression.Constant(MaxDate, typeof(DateTime?));

            var ex1 = Expression.LessThanOrEqual(getter.Body, maxDate);
            var ex2 = Expression.GreaterThanOrEqual(getter.Body, minDate);

            var ex = Expression.AndAlso(ex1, ex2);

            return Expression.Lambda<Func<T, bool>>(ex, entity);

            //var g = getter.Compile();
            //var minDate = MinDate;
            //var maxDate = MaxDate;

            //return t => g(t) <= maxDate && g(t) >= minDate;
        }

        private IProperty<bool> _updateTrigger = H.Property<bool>(c => c
            .On(e => e.MinDate)
            .On(e => e.MaxDate)
            .On(e => e.Update)
            .On(e => e.Enabled)
            .NotNull(e => e.Update)
            .Do((e,p) => e.Update.Invoke())
        );

        public FilterDateViewModel Link<T>(ObservableQuery<T> q, Expression<Func<T, DateTime?>> getter)
            where T : class, IEntity
        {
            //var entity = getter.Parameters[0];
            q.AddFilter(Title,()=> Match(getter));
            Update = ()=>q.UpdateAsync();
            return this;
        }

        private static DateTime Shift(DateTime referenceDate, int shift, DateShiftUnit unit)
        {
            switch (unit)
            {
                case DateShiftUnit.Day:
                    return referenceDate.AddDays(shift);
                case DateShiftUnit.Week:
                    return referenceDate.AddDays(shift*7);
                case DateShiftUnit.Month:
                    return referenceDate.AddMonths(shift);
                case DateShiftUnit.Year:
                    return referenceDate.AddYears(shift);
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private static (int, DateShiftUnit) CalculateShift(DateTime referenceDate, DateTime date)
        {
            DateShiftUnit unit = DateShiftUnit.Day;

            Func<DateTime, DateTime> f = d => d.AddDays(1);
            int direction = (referenceDate <= date) ? 1 : -1;

            if (referenceDate.DayOfWeek == date.DayOfWeek)
            {
                unit = DateShiftUnit.Week;
                f = d => d.AddDays(7);
            }

            if (referenceDate.Day == date.Day)
            {
                unit = DateShiftUnit.Month;
                f = d => d.AddMonths(1);
            }

            if (referenceDate.DayOfYear == date.DayOfYear)
            {
                unit = DateShiftUnit.Year;
                f = d => d.AddYears(1);
            }

            int value = 0;
            var dateMax = (date > referenceDate) ? date : referenceDate;
            var dateMin = (referenceDate > date) ? date : referenceDate;
            while (dateMax > dateMin)
            {
                value+=direction;
                dateMin = f(dateMin);
            }

            return (value, unit);
        }
    }
}