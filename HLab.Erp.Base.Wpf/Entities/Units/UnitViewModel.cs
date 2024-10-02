using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Windows.Automation.Text;
using System.Windows.Input;
using System.Xml;
using HLab.Base.ReactiveUI;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Mvvm.Annotations;
using ReactiveUI;


namespace HLab.Erp.Base.Wpf.Entities.Units;

public class UnitViewModel: ListableEntityViewModel<Unit>
{
    public UnitViewModel(Injector i) : base(i)
    {
        _iconPath = this
            .WhenAnyValue(e => e.Model)
            .Select(m => m?.IconPath)
            .ToProperty(this, e => e.IconPath);

        this.WhenAnyValue(e => e.Model)
            .Subscribe(m => _testUnit = m);

        _testResult = this
            .WhenAnyValue(
                e => e.Model,
                e => e.TestUnit,
                e => e.TestValue,
                e => e.Model.Abs,
                e => e.Model.OffsetA,
                e => e.Model.OffsetB,
                selector : (model, testUnit, testValue, abs, offsetA, offsetB) => model?.Qty(testUnit.AbsQty(testValue))

            )
            .Select(_ => TestResult)
            .ToProperty(this, e => e.TestResult);
    }

    //public string SubTitle => _subTitle.Get();
    //private string _subTitle = H.Property<string>(c => c
    //    .Set(e => e.GetSubTitle )
    //    .On(e => e.Model.Name)
    //    .Update()
    //);
    //private string GetSubTitle => $"{Model?.Variant}\n{Model?.Form?.Name}";


    public override string IconPath => _iconPath.Value;
    readonly ObservableAsPropertyHelper<string> _iconPath;

    string GetIconPath => Model?.IconPath??Model?.IconPath??base.IconPath;


    public double TestValue
    {
        get => _testValue;
        set => this.SetAndRaise(ref _testValue,value);
    }

    double _testValue = 1.0;

    public Unit TestUnit
    {
        get => _testUnit;
        set => this.SetAndRaise(ref _testUnit,value);
    }
    Unit _testUnit;

    public double TestResult => _testResult.Value;
    readonly ObservableAsPropertyHelper<double> _testResult;
}

public class UnitViewModelDesign() : UnitViewModel(null), IDesignViewModel
{
    public new Unit Model { get; } = Unit.DesignModel;
}