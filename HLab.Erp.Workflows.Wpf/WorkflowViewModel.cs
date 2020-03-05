using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using HLab.Mvvm;
using HLab.Notify.Annotations;

namespace HLab.Erp.Workflows
{
    public class WorkflowViewModel:ViewModel<WorkflowViewModel,IWorkflow>
    {

        public ObservableCollection<WorkflowAction> BackwardActions { get; } = new ObservableCollection<WorkflowAction>();
        public ObservableCollection<WorkflowAction> Actions { get; } = new ObservableCollection<WorkflowAction>();

        [TriggerOn(nameof(Model))]
        public void UpdateActions()
        {
            Actions_CollectionChanged(null,null);
            Model.Actions.CollectionChanged += Actions_CollectionChanged;
        }

        private object _lock = new object();
        private void Actions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
            () =>
                {
                    lock(_lock)
                    {
                        Actions.Clear();
                        BackwardActions.Clear();
                        foreach (var m in Model.Actions)
                        {
                            switch (m.Direction)
                            {
                                case WorkflowDirection.Forward:
                                    if(Actions.Any(a => a.Caption == m.Caption))
                                    { }
                                    else Actions.Add(m);
                                    break;
                                case WorkflowDirection.Backward:
                                    BackwardActions.Add(m);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }
            );
        }
    }
}