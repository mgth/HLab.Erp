using System.Collections.ObjectModel;
using HLab.Erp.Workflows;
using HLab.Mvvm;
using HLab.Notify.Annotations;

namespace HLab.Erp.Workflow
{
    public class WorkflowViewModel:ViewModel<WorkflowViewModel,IWorkflow>
    {

        public ObservableCollection<WorkflowAction> BackwardActions = new ObservableCollection<WorkflowAction>();
        public ObservableCollection<WorkflowAction> Actions = new ObservableCollection<WorkflowAction>();

        [TriggerOn(nameof(Model))]
        public void UpdateActions()
        {
            Model.Actions.CollectionChanged += Actions_CollectionChanged;
        }

        private void Actions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Actions.Clear();
            foreach (var m in Model.Actions)
                Actions.Add(m);
        }
    }
}