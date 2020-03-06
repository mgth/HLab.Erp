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
        private ObservableCollection<WorkflowAction> _backwardActions = new ObservableCollection<WorkflowAction>();
        private ObservableCollection<WorkflowAction> _actions = new ObservableCollection<WorkflowAction>();
        public ReadOnlyObservableCollection<WorkflowAction> BackwardActions { get; }
        public ReadOnlyObservableCollection<WorkflowAction> Actions { get; }

        public WorkflowViewModel()
        {
            BackwardActions = new ReadOnlyObservableCollection<WorkflowAction>(_backwardActions);
            Actions = new ReadOnlyObservableCollection<WorkflowAction>(_actions);
        }


        [TriggerOn(nameof(Model))]
        public void UpdateActions()
        {
            Actions_CollectionChanged(null,null);
            ((INotifyCollectionChanged)Model.Actions).CollectionChanged += Actions_CollectionChanged;
        }

        private object _lock = new object();
        private void Actions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
            () =>
                {
                    lock(_lock)
                    {
                        _actions.Clear();
                        _backwardActions.Clear();
                        foreach (var m in Model.Actions)
                        {
                            switch (m.Direction)
                            {
                                case WorkflowDirection.Forward:
                                    _actions.Add(m);
                                    break;
                                case WorkflowDirection.Backward:
                                    _backwardActions.Add(m);
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