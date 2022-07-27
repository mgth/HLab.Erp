using System;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.ListFilters
{
    public static class DateFilterViewModelExtension
    {
        public static IColumnConfigurator<T, DateTime?, DateFilterNullable> MaxDate<T>(this IColumnConfigurator<T, DateTime?,DateFilterNullable> tc, DateTime date) where T : class, IEntity, new()
        {
            tc.Filter.MaxDate = date;
            return tc;
        }

        public static IColumnConfigurator<T, DateTime?, DateFilterNullable> MinDate<T>(this IColumnConfigurator<T, DateTime?, DateFilterNullable> tc, DateTime date) where T : class, IEntity, new()
        {
            tc.Filter.MinDate = date;
            return tc;
        }
        public static IColumnConfigurator<T, DateTime, DateFilter> MaxDate<T>(this IColumnConfigurator<T, DateTime,DateFilter> tc, DateTime date) where T : class, IEntity, new()
        {
            tc.Filter.MaxDate = date;
            return tc;
        }

        public static IColumnConfigurator<T, DateTime, DateFilter> MinDate<T>(this IColumnConfigurator<T, DateTime, DateFilter> tc, DateTime date) where T : class, IEntity, new()
        {
            tc.Filter.MinDate = date;
            return tc;
        }
    }
    public class DateFilter : DateFilter<DateTime>
    {
    }

    public class DateFilterNullable : DateFilter<DateTime?>
    {
    }

    public abstract class DateFilter<TDate> : Filter<TDate>
    {
        protected DateFilter() => H<DateFilter<TDate>>.Initialize(this);

        
        public DateTime ReferenceDate {
            get => _referenceDate.Get();
            set => _referenceDate.Set(value);
        }

        readonly IProperty<DateTime> _referenceDate = H<DateFilter<TDate>>.Property<DateTime>();

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

        readonly IProperty<DateTime> _minDate = H<DateFilter<TDate>>.Property<DateTime>(c => c
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

        readonly IProperty<bool> _minDateCalculated = H<DateFilter<TDate>>.Property<bool>(c => c.Default(false));

        public bool MinDateEnabled => _minDateEnabled.Get();

        readonly IProperty<bool> _minDateEnabled = H<DateFilter<TDate>>.Property<bool>(c => c
            .Default(true)
            .On(e => e.MinDateCalculated)
            .Set(e => !e.MinDateCalculated));

        public int MinDateShift
        {
            get => _minDateShift.Get();
            set => _minDateShift.Set(value);
        }

        readonly IProperty<int> _minDateShift = H<DateFilter<TDate>>.Property<int>();

        readonly ITrigger _triggerMinDateShift = H<DateFilter<TDate>>.Trigger(c => c
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

        readonly IProperty<DateShiftUnit> _minDateShiftUnit = H<DateFilter<TDate>>.Property<DateShiftUnit>();

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

        readonly IProperty<DateTime> _maxDate = H<DateFilter<TDate>>.Property<DateTime>(c => c
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

        readonly IProperty<bool> _maxDateCalculated = H<DateFilter<TDate>>.Property<bool>(c => c.Default(false));

        public bool MaxDateEnabled => _maxDateEnabled.Get();

        readonly IProperty<bool> _maxDateEnabled = H<DateFilter<TDate>>.Property<bool>(c => c
            .Default(true)
            .On(e => e.MaxDateCalculated)
            .Set(e => !e.MaxDateCalculated));

        public int MaxDateShift
        {
            get => _maxDateShift.Get();
            set => _maxDateShift.Set(value);
        }

        readonly IProperty<int> _maxDateShift = H<DateFilter<TDate>>.Property<int>();

        readonly ITrigger _triggerMaxDateShift = H<DateFilter<TDate>>.Trigger(c => c
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

        readonly IProperty<DateShiftUnit> _maxDateShiftUnit = H<DateFilter<TDate>>.Property<DateShiftUnit>();



        public override Expression<Func<T, bool>> Match<T>(Expression<Func<T, TDate>> getter)
        {
            var entity = getter.Parameters[0];
            var minDate = Expression.Constant(MinDate.ToUniversalTime(), typeof(TDate));
            var maxDate = Expression.Constant(MaxDate.ToUniversalTime(), typeof(TDate));

            var ex1 = Expression.LessThanOrEqual(getter.Body, maxDate);
            var ex2 = Expression.GreaterThanOrEqual(getter.Body, minDate);

            var ex = Expression.AndAlso(ex1, ex2);

            return Expression.Lambda<Func<T, bool>>(ex, entity);

            //var g = getter.Compile();
            //var minDate = MinDate;
            //var maxDate = MaxDate;

            //return t => g(t) <= maxDate && g(t) >= minDate;
        }

        ITrigger _updateTrigger = H<DateFilter<TDate>>.Trigger(c => c
            .On(e => e.MinDate)
            .On(e => e.MaxDate)
            .On(e => e.Update)
            .On(e => e.Enabled)
            .NotNull(e => e.Update)
            .Do((e,p) => e.Update.Invoke())
        );


        static DateTime Shift(DateTime referenceDate, int shift, DateShiftUnit unit)
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

        static (int, DateShiftUnit) CalculateShift(DateTime referenceDate, DateTime date)
        {
            var unit = DateShiftUnit.Day;

            Func<DateTime, DateTime> f = d => d.AddDays(1);
            var direction = (referenceDate <= date) ? 1 : -1;

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

        public override Func<TSource, bool> PostMatch<TSource>(Func<TSource, TDate> getter)
        {
            throw new NotImplementedException();
        }

        public override XElement ToXml()
        {
            var element = base.ToXml();

            element.SetAttributeValue("MinDate",MinDate.ToString());
            element.SetAttributeValue("MaxDate",MaxDate.ToString());

            return element;
        }

        public override void FromXml(XElement element)
        {
            GetXmlDate(element,"MinDate",d => MinDate = d);
            GetXmlDate(element,"MaxDate",d => MaxDate = d);
        }

        static void GetXmlDate(XElement element, string Name, Action<DateTime> setter)
        {
            var attribute = element.Attribute("MinDate");
            if (attribute != null)
            {
                if(DateTime.TryParse(attribute.Value, out var date))
                {
                    setter(date);
                }
            }
        }

    }
}