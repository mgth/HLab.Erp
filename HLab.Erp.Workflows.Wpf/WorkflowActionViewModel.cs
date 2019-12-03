using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{
    class WorkflowActionViewModel : ViewModel<WorkflowActionViewModel, WorkflowAction>
    {
        public string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
            .OneWayBind(e => e.Model.IconPath)
        );

        public ICommand Command { get; } = H.Command(c => c
            .CanExecute(vm => vm.Model.Check() == WorkflowConditionResult.Passed)
            .Action(e=> e.Model.Action())
        );
    }
}
