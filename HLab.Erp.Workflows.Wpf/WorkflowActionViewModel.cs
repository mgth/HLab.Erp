using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{
    class WorkflowActionViewModel : ViewModel<WorkflowActionViewModel, WorkflowAction>
    {
        [Import]
        private IIconService _icons;
        public object Icon => _icon.Get();
        private readonly IProperty<object> _icon = H.Property<object>(c => c
            .Set(vm => vm._icons.GetIcon(vm.Model.Icon) )
        );

        public ICommand Command { get; } = H.Command(c => c
            .CanExecute(vm => vm.Model.Check() == WorkflowConditionResult.Passed)
            .Action(e=> e.Model.Action())
        );
    }
}
