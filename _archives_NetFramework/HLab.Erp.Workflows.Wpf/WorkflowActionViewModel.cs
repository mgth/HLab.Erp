using HLab.Mvvm;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace Erp.Workflow.Wpf
{
    class WorkflowActionViewModel : ViewModel<WorkflowActionViewModel, WorkflowAction>
    {
        [Import]
        private IIconService _icons;
        public object Icon => _icon.Get();
        private readonly IProperty<object> _icon = H.Property<object>(c => c
            .Set(vm => vm._icons.GetIcon(vm.Model.Icon) )
        );

        public ICommand Command => _command.Get();
        private readonly IProperty<ICommand> _command = H.Property<ICommand>(c => c
            .Command(
                (vm,o)=> vm.Model.Action(),
                vm => vm.Model.Check() == WorkflowConditionResult.Passed)
        );
    }
}
