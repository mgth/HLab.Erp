using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Xml.Linq;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Data;
using ReactiveUI;

namespace HLab.Erp.Core.ListFilters
{
    public static class DateFilterViewModelExtension
    {
        public static IColumnConfigurator<T, DateTime?, DateFilterNullable> MaxDate<T>(this IColumnConfigurator<T, DateTime?,DateFilterNullable> @this, DateTime date) where T : class, IEntity, new() 
            => @this.Build(b => b.Filter.MaxDate = date);

        public static IColumnConfigurator<T, DateTime?, DateFilterNullable> MinDate<T>(this IColumnConfigurator<T, DateTime?, DateFilterNullable> @this, DateTime date) where T : class, IEntity, new() 
            => @this.Build(b => b.Filter.MinDate = date);

        public static IColumnConfigurator<T, DateTime, DateFilter> MaxDate<T>(this IColumnConfigurator<T, DateTime,DateFilter> @this, DateTime date) where T : class, IEntity, new() 
            => @this.Build(b => b.Filter.MaxDate = date);

        public static IColumnConfigurator<T, DateTime, DateFilter> MinDate<T>(this IColumnConfigurator<T, DateTime, DateFilter> @this, DateTime date) where T : class, IEntity, new()
            => @this.Build(b => b.Filter.MinDate = date);
    }
    public class DateFilter : DateFilter<DateTime>
    {
    }

    public class DateFilterNullable : DateFilter<DateTime?>
    {
    }

    public abstract class DateFilter<TDate> : Filter<TDate>
    {
        protected DateFilter()
        {
            _minDateEnabled = this.WhenAnyValue(
                e => e.MinDateCalculated, 
                selector:e => !e)
            .ToProperty(this, nameof(MinDateEnabled));

            this.WhenAnyValue(
                    e => e.MinDateCalculated,
                    e => e.ReferenceDate,
                    e => e.MinDateShift,
                    e => e.MinDateShiftUnit
                )
                .Subscribe((e) =>
                {
                    if (e.Item1) MinDate = Shift(e.Item2, e.Item3, e.Item4);
                });

            this.WhenAnyValue(
                    e => e.MaxDateCalculated,
                    e => e.ReferenceDate,
                    e => e.MaxDateShift,
                    e => e.MaxDateShiftUnit
                )
                .Subscribe((e) =>
                {
                    if (e.Item1) MinDate = Shift(e.Item2, e.Item3, e.Item4);
                });


            this.WhenAnyValue(
                    e => e.MinDateCalculated,
                    e => e.ReferenceDate,
                    e => e.MinDate
                )
                .Where(e => !e.Item1)
                .Subscribe(e =>
                {
                    (MinDateShift, MinDateShiftUnit) = CalculateShift(e.Item2, e.Item3);
                });

            this.WhenAnyValue(
                    e => e.MaxDateCalculated,
                    e => e.ReferenceDate,
                    e => e.MaxDate
                )
                .Where(e => !e.Item1)
                .Subscribe(e =>
                {
                    (MaxDateShift, MaxDateShiftUnit) = CalculateShift(e.Item2, e.Item3);
                });

            _maxDateEnabled = this
                .WhenAnyValue(
                    e => e.MaxDateCalculated, 
                    selector: e => !e)
                .ToProperty(this, nameof(MaxDateEnabled));

            //ITrigger _updateTrigger = H<DateFilter<TDate>>.Trigger(c => c
            //    .On(e => e.MinDate)
            //    .On(e => e.MaxDate)
            //    .On(e => e.Update)
            //    .On(e => e.Enabled)
            //    .NotNull(e => e.Update)
            //    .Do((e,p) => e.Update.Invoke())

            this.WhenAnyValue(
                e => e.MinDate,
                e => e.MaxDate,
                e => e.Update,
                e => e.Enabled)
            .Subscribe(e => Update?.Invoke());


        }

        
        public DateTime ReferenceDate {
            get => _referenceDate;
            set => this.RaiseAndSetIfChanged(ref _referenceDate,value);
        }

        DateTime _referenceDate;

        /// <summary>
        /// Min Date
        /// </summary>
        public DateTime MinDate
        {
            get => _minDate;
            set
            {
                MinDateCalculated = false;
                this.RaiseAndSetIfChanged(ref _minDate, value);
            }
        }
        DateTime _minDate;

        public bool MinDateCalculated {
            get => _minDateCalculated;
            set => this.RaiseAndSetIfChanged(ref _minDateCalculated,value);
        }

        bool _minDateCalculated = false;

        public bool MinDateEnabled => _minDateEnabled.Value;

        readonly ObservableAsPropertyHelper<bool> _minDateEnabled;

        public int MinDateShift
        {
            get => _minDateShift;
            set => this.RaiseAndSetIfChanged(ref _minDateShift,value);
        }

        int _minDateShift;


        public DateShiftUnit MinDateShiftUnit
        {
            get => _minDateShiftUnit;
            set => this.RaiseAndSetIfChanged(ref _minDateShiftUnit,value);
        }

        DateShiftUnit _minDateShiftUnit;

        /// <summary>
        /// Max Date
        /// </summary>
        public DateTime MaxDate
        {
            get => _maxDate;
            set
            {
                MaxDateCalculated = false;
                this.RaiseAndSetIfChanged(ref _maxDate,value);
            }
        }
        DateTime _maxDate;

        public bool MaxDateCalculated {
            get => _maxDateCalculated;
            set => this.RaiseAndSetIfChanged(ref _maxDateCalculated,value);
        }

        bool _maxDateCalculated = false;

        public bool MaxDateEnabled => _maxDateEnabled.Value;
        readonly ObservableAsPropertyHelper<bool> _maxDateEnabled;

        public int MaxDateShift
        {
            get => _maxDateShift;
            set => this.RaiseAndSetIfChanged(ref _maxDateShift,value);
        }

        int _maxDateShift;


        public DateShiftUnit MaxDateShiftUnit
        {
            get => _maxDateShiftUnit;
            set => this.RaiseAndSetIfChanged(ref _maxDateShiftUnit,value);
        }

        DateShiftUnit _maxDateShiftUnit;



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