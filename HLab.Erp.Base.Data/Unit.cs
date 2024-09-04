using System;
using System.Linq.Expressions;
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
        public Unit() { }

        public string Name
        {
            get => _name; set => this.RaiseAndSetIfChanged(ref _name,value);
        }

        string _name = "";

        public string Symbol
        {
            get => _symbol; set => this.RaiseAndSetIfChanged(ref _symbol,value);
        }

        string _symbol = "";

        public double Coefficient
        {
            get => _coefficient;
            set => this.RaiseAndSetIfChanged(ref _coefficient,value);
        }

        double _coefficient = 1.0;
        public double OffsetA
        {
            get => _offsetA;
            set => this.RaiseAndSetIfChanged(ref _offsetA,value);
        }

        double _offsetA = 0.0;
        public double OffsetB
        {
            get => _offsetB;
            set => this.RaiseAndSetIfChanged(ref _offsetB,value);
        }

        double _offsetB = 0.0;

        public int Exponent
        {
            get => _exponent;
            set => this.RaiseAndSetIfChanged(ref _exponent,value);
        }

        int _exponent = 0;

        public int? UnitClassId
        {
            get => _unitClass.Id;
            set => this.RaiseAndSetIfChanged(ref _unitClass.Id,value);
        }

        [Ignore]
        public UnitClass UnitClass
        {
            get => _unitClass;
            set => this.RaiseAndSetIfChanged(ref _unitClass,value);
        }

        readonly IForeign<UnitClass> _unitClass = H.Foreign<UnitClass>();


        public bool Default
        {
            get => _default; 
            set => this.RaiseAndSetIfChanged(ref _default,value);
        }

        bool _default = false;

        public double DefaultQty
        {
            get => _defaultQty; 
            set => this.RaiseAndSetIfChanged(ref _defaultQty,value);
        }

        double _defaultQty = 1.0;

        public bool IsRatio
        {
            get => _isRatio; 
            set => this.RaiseAndSetIfChanged(ref _isRatio,value);
        }

        bool _isRatio = false;

        [Ignore]
        public string Caption => _caption.Get();

        string _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .Set(e => string.IsNullOrWhiteSpace(e.Name) ? "{New Unit}" : $"{e.Name}")
        );

        [Ignore]
        public string IconPath => UnitClass?.IconPath??"";

        [Ignore]
        public double Abs => _abs.Get();

        double _abs = H.Property<double>(c => c
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

        ITrigger _update = H.Trigger(c => c
            .On(e => e.OffsetA)
            .On(e => e.OffsetB)
            .On(e => e.Coefficient)
            .On(e => e.Exponent)
            .Do(e => e.Compile())
        );

        void Compile()
        {
            _absQty = ExpressionAbsQty().Compile();
            _qty = ExpressionQty().Compile();
        }

        public double AbsQty(double qty) => Math.Round(_absQty(qty),11); //( qty + OffsetA ) * Abs + OffsetB;
        Func<double, double> _absQty = d => d;

        Expression<Func<double, double>> ExpressionAbsQty()
        {
            var qty = Expression.Parameter(typeof(double), "qty");
            var offsetA = Expression.Constant(OffsetA);
            var offsetB = Expression.Constant(OffsetB);
            var abs = Math.Pow(Coefficient, Exponent);
            var absExpression = Expression.Constant(abs);

            Expression e = OffsetA == 0 ? qty : Expression.Add(qty, offsetA);
            e = Math.Abs(abs - 1.0) < double.Epsilon ? e : Expression.Multiply(e, absExpression);

            e = abs == 0 ? offsetB : OffsetB == 0 ? e : Expression.Add(e,offsetB);

            return Expression.Lambda<Func<double, double>>(e,qty);
        }

        public double Qty(double absQty) => Math.Round(_qty(absQty),11);//(absQty - OffsetB) / Abs - OffsetA;
        Func<double, double> _qty = d => d;

        Expression<Func<double, double>> ExpressionQty()
        {
            var absQty = Expression.Parameter(typeof(double), "absQty");
            var offsetA = Expression.Constant(OffsetA);
            var offsetB = Expression.Constant(OffsetB);
            var abs = Math.Pow(Coefficient, Exponent);
            var absExpression = Expression.Constant(abs);

            Expression e = OffsetB == 0 ? absQty : Expression.Subtract(absQty, offsetB);
            e = Math.Abs(abs - 1.0) < double.Epsilon ? e : Expression.Divide(e, absExpression);
            e = OffsetA == 0 ? e : Expression.Subtract(e, offsetA);

            return Expression.Lambda<Func<double, double>>(e,absQty);
        }

    }
}
