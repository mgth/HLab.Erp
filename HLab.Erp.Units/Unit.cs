using HLab.Erp.Data;
using System;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Units
{
    using H = HD<Unit>;
    public class Unit : Entity
    {
        public Unit() => H.Initialize(this);

        public string Name
        {
            get => _name.Get(); set => _name.Set(value);
        }

        private readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));

        public string Symbol
        {
            get => _symbol.Get(); set => _symbol.Set(value);
        }

        private readonly IProperty<string> _symbol = H.Property<string>(c => c.Default(""));

        public double Coefficient
        {
            get => _coefficient.Get();
            set => _coefficient.Set(value);
        }

        private readonly IProperty<double> _coefficient = H.Property<double>(c => c.Default(1.0));

        public int Exponent
        {
            get => _exponent.Get();
            set => _exponent.Set(value);
        }

        private readonly IProperty<int> _exponent = H.Property<int>(c => c.Default(1));

        public string Group
        {
            get => _group.Get();
            set => _group.Set(value);
        }

        private readonly IProperty<string> _group = H.Property<string>(c => c.Default(""));


        public bool Default
        {
            get => _default.Get(); set => _default.Set(value);
        }

        private readonly IProperty<bool> _default = H.Property<bool>(c => c.Default(false));

        public double DefaultQty
        {
            get => _defaultQty.Get(); set => _defaultQty.Set(value);
        }

        private readonly IProperty<double> _defaultQty = H.Property<double>(c => c.Default(1.0));

        public bool IsRatio
        {
            get => _isRatio.Get(); set => _isRatio.Set(value);
        }

        private readonly IProperty<bool> _isRatio = H.Property<bool>(c => c.Default(false));

        /// <summary>
        /// returns abs coef for qty : 
        /// AbsQty = unitQty * Abs
        /// UnitQty = AbsQty / Abs
        /// </summary>
        [TriggerOn(nameof(Coefficient))]
        [TriggerOn(nameof(Exponent)), Ignore]
        public double Abs => Math.Pow(Coefficient, Exponent);
        public double AbsQty(double qty) => qty * Abs;
        public double Qty(double absQty) => absQty / Abs;

    }
}
