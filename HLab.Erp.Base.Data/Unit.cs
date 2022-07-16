using System;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Base.Data
{
    using H = HD<Unit>;
    public class Unit : Entity, IListableModel
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
        public double Offset
        {
            get => _offset.Get();
            set => _offset.Set(value);
        }
        private readonly IProperty<double> _offset = H.Property<double>(c => c.Default(0.0));

        public int Exponent
        {
            get => _exponent.Get();
            set => _exponent.Set(value);
        }
        private readonly IProperty<int> _exponent = H.Property<int>(c => c.Default(0));

        public int? UnitClassId
        {
            get => _unitClass.Id.Get();
            set => _unitClass.Id.Set(value);
        }

        [Ignore]
        public UnitClass UnitClass
        {
            get => _unitClass.Get();
            set => _unitClass.Set(value);
        }
        private readonly IForeign<UnitClass> _unitClass = H.Foreign<UnitClass>();


        public bool Default
        {
            get => _default.Get(); 
            set => _default.Set(value);
        }
        private readonly IProperty<bool> _default = H.Property<bool>(c => c.Default(false));

        public double DefaultQty
        {
            get => _defaultQty.Get(); 
            set => _defaultQty.Set(value);
        }
        private readonly IProperty<double> _defaultQty = H.Property<double>(c => c.Default(1.0));

        public bool IsRatio
        {
            get => _isRatio.Get(); 
            set => _isRatio.Set(value);
        }
        private readonly IProperty<bool> _isRatio = H.Property<bool>(c => c.Default(false));

        [Ignore]
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .Set(e => string.IsNullOrWhiteSpace(e.Name) ? "{New Unit}" : $"{e.Name}")
        );

        [Ignore]
        public string IconPath => UnitClass?.IconPath??"";

        [Ignore]
        public double Abs => _abs.Get();

        private readonly IProperty<double> _abs = H.Property<double>(
            c => c
                .Set(e => Math.Pow(e.Coefficient, e.Exponent))
                .On(e => e.Coefficient)
                .On(e => e.Exponent)
                .Update()
            );
        public static Unit DesignModel => new(){
            Name = "mg",
            UnitClass = Data.UnitClass.DesignModel,
            DefaultQty = 1,
            Exponent = -3,
            };

        public double AbsQty(double qty) => qty * Abs + Offset;
        public double Qty(double absQty) => (absQty - Offset) / Abs;

    }
}
