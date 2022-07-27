using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Automation.Text;
using System.Windows.Input;
using System.Xml;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf.Entities.Units
{
    using H = H<UnitViewModel>;

    public class UnitViewModel: ListableEntityViewModel<Unit>
    {
        public UnitViewModel(Injector i) : base(i)
        {
            H.Initialize(this);
        }

        //public string SubTitle => _subTitle.Get();
        //private readonly IProperty<string> _subTitle = H.Property<string>(c => c
        //    .Set(e => e.GetSubTitle )
        //    .On(e => e.Model.Name)
        //    .Update()
        //);
        //private string GetSubTitle => $"{Model?.Variant}\n{Model?.Form?.Name}";


        public override string IconPath => _iconPath.Get();

        readonly IProperty<string> _iconPath = H.Property<string>(c => c
        .Set(e => e.GetIconPath )
        .On(e => e.Model.IconPath)
        .Update()
        );

        string GetIconPath => Model?.IconPath??Model?.IconPath??base.IconPath;


        public double TestValue
        {
            get => _testValue.Get();
            set => _testValue.Set(value);
        }

        readonly IProperty<double> _testValue = H.Property<double>(c => c.Default(1.0));

        public Unit TestUnit
        {
            get => _testUnit.Get();
            set => _testUnit.Set(value);
        }

        readonly IProperty<Unit> _testUnit = H.Property<Unit>(c => c.Set(e => e.Model));

        public double TestResult => _testResult.Get();

        readonly IProperty<double> _testResult = H.Property<double>(c => c
            .NotNull(e => e.TestUnit)
            .NotNull(e => e.TestValue)
            .NotNull(e => e.Model)
            .Set(e => e.Model.Qty(e.TestUnit.AbsQty(e.TestValue)))
            .On(e => e.TestUnit)
            .On(e => e.TestValue)
            .On(e => e.Model.Abs)
            .On(e => e.Model.OffsetA)
            .On(e => e.Model.OffsetB)
            .Update()
            );

    }
    public class UnitViewModelDesign : UnitViewModel, IViewModelDesign
    {
        public UnitViewModelDesign():base(null)
        {
        }

        public new Unit Model { get; } = Unit.DesignModel;
    }
}
