using System;
using System.Linq;
using System.Text.RegularExpressions;
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

        //public string SubTitle => _subTitle.Get();
        //private readonly IProperty<string> _subTitle = H.Property<string>(c => c
        //    .Set(e => e.GetSubTitle )
        //    .On(e => e.Model.Name)
        //    .Update()
        //);
        //private string GetSubTitle => $"{Model?.Variant}\n{Model?.Form?.Name}";


        public override string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
        .Set(e => e.GetIconPath )
        .On(e => e.Model.IconPath)
        .Update()
        );

        private string GetIconPath => Model?.IconPath??Model?.IconPath??base.IconPath;



        //public ProductWorkflow Workflow => _workflow.Get();
        //private readonly IProperty<ProductWorkflow> _workflow = H.Property<ProductWorkflow>(c => c
        //    .On(e => e.Model)
        //    .OnNotNull(e => e.Locker)
        //    .Set(vm => new ProductWorkflow(vm.Model,vm.Locker))
        //);
    }
    public class UnitViewModelDesign : UnitViewModel, IViewModelDesign
    {
        public UnitViewModelDesign()
        {
        }

        public new Unit Model { get; } = Unit.DesignModel;
    }
}
