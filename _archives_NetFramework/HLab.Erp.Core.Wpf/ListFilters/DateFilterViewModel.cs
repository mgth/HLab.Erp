using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using System;
using System.Threading;

namespace HLab.Erp.Core.Wpf.ListFilters
{
    public enum DateShiftUnit
    {
        Day,Week,Month,Year
    }
    public class DateFilterViewModel : FilterViewModel
    {

        private new class H : NotifyHelper<DateFilterViewModel> { }

        private Action _update = null;
 
        public DateFilterViewModel()
        {
            H.Initialize(this);
        }

        public DateTime ReferenceDate {
            get => _referenceDate.Get();
            set => _referenceDate.Set(value);
        }
        private readonly IProperty<DateTime> _referenceDate = H.Property<DateTime>();

        /// <summary>
        /// Min Date
        /// </summary>
        public DateTime MinDate {
            get => _minDate.Get();
            set => _minDate.Set(value);
        }
        private readonly IProperty<DateTime> _minDate = H.Property<DateTime>();
        public bool MinDateCalculated {
            get => _minDateCalculated.Get();
            set => _minDateCalculated.Set(value);
        }
        private readonly IProperty<bool> _minDateCalculated = H.Property<bool>();

        public bool MinDateEnabled => _minDateEnabled.Get();
        private readonly IProperty<bool> _minDateEnabled = H.Property<bool>(c => c.On(e => e.MinDateCalculated).Set(e => !e.MinDateCalculated));

        public int MinDateShift
        {
            get => _minDateShift.Get();
            set => _minDateShift.Set(value);
        }
        private readonly IProperty<int> _minDateShift = H.Property<int>();

        [TriggerOn(nameof(MinDateCalculated))]
        [TriggerOn(nameof(MinDateShift))]
        [TriggerOn(nameof(MinDateShiftUnit))]
        private void SetMinDate()
        {
            if(MinDateCalculated)
                MinDate = Shift(ReferenceDate, MinDateShift, MinDateShiftUnit);
        }
        [TriggerOn(nameof(MinDateCalculated))]
        [TriggerOn(nameof(MinDate))]
        private void SetMinDateShift()
        {
            if (!MinDateCalculated)
                (MinDateShift, MinDateShiftUnit) = CalculateShift(ReferenceDate, MinDate);
        }

        public DateShiftUnit MinDateShiftUnit
        {
            get => _minDateShiftUnit.Get();
            set => _minDateShiftUnit.Set(value);
        }
        private readonly IProperty<DateShiftUnit> _minDateShiftUnit = H.Property<DateShiftUnit>();

        /// <summary>
        /// Max Date
        /// </summary>
        public DateTime MaxDate {
            get => _maxDate.Get();
            set => _maxDate.Set(value);
        }
        private readonly IProperty<DateTime> _maxDate = H.Property<DateTime>();
        public bool MaxDateCalculated {
            get => _maxDateCalculated.Get();
            set => _maxDateCalculated.Set(value);
        }
        private readonly IProperty<bool> _maxDateCalculated = H.Property<bool>();
        public bool MaxDateEnabled => _maxDateEnabled.Get();
        private readonly IProperty<bool> _maxDateEnabled = H.Property<bool>(c => c.On(e => e.MaxDateCalculated).Set(e => !e.MaxDateCalculated));

        public int MaxDateShift
        {
            get => _maxDateShift.Get();
            set => _maxDateShift.Set(value);
        }

        [TriggerOn(nameof(MaxDateCalculated))]
        [TriggerOn(nameof(MaxDateShift))]
        [TriggerOn(nameof(MaxDateShiftUnit))]
        private void SetMaxDate()
        {
            if(MaxDateCalculated)
                MaxDate = Shift(ReferenceDate, MaxDateShift, MaxDateShiftUnit);
        }

        [TriggerOn(nameof(MaxDateCalculated))]
        [TriggerOn(nameof(MaxDate))]
        private void SetMaxDateShift()
        {
            if(!MaxDateCalculated)
                (MaxDateShift,MaxDateShiftUnit) = CalculateShift(ReferenceDate, MaxDate);
        }

        private readonly IProperty<int> _maxDateShift = H.Property<int>();
        public DateShiftUnit MaxDateShiftUnit
        {
            get => _maxDateShiftUnit.Get();
            set => _maxDateShiftUnit.Set(value);
        }
        private readonly IProperty<DateShiftUnit> _maxDateShiftUnit = H.Property<DateShiftUnit>();








        public bool Match(DateTime? date) => date<=MaxDate && date>=MinDate;

        [TriggerOn(nameof(MinDate))]
        [TriggerOn(nameof(MaxDate))]
        void Update()
        {
            _update?.Invoke();
        }

        public DateFilterViewModel Link<T>(ObservableQuery<T> q, Func<T, DateTime?> getter)
        where T : class, IEntity
        {
            q.AddFilter(Title, s => Match(getter(s)));
            _update = q.Update;
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
