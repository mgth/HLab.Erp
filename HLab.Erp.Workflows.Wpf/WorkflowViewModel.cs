using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using HLab.Mvvm;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{
    using H = H<WorkflowViewModel>;
    public class WorkflowViewModel:ViewModel<IWorkflow>
    {
        private readonly ObservableCollection<WorkflowAction> _backwardActions = new ObservableCollection<WorkflowAction>();
        private readonly ObservableCollection<WorkflowAction> _actions = new ObservableCollection<WorkflowAction>();
        private readonly ObservableCollection<string> _highlights = new ObservableCollection<string>();
        public ReadOnlyObservableCollection<WorkflowAction> BackwardActions { get; }
        public ReadOnlyObservableCollection<WorkflowAction> Actions { get; }
        public ReadOnlyObservableCollection<string> Highlights { get; }

        public WorkflowViewModel()
        {
            BackwardActions = new ReadOnlyObservableCollection<WorkflowAction>(_backwardActions);
            Actions = new ReadOnlyObservableCollection<WorkflowAction>(_actions);
            Highlights = new ReadOnlyObservableCollection<string>(_highlights);

            H.Initialize(this);
        }


        [TriggerOn(nameof(Model))]
        public void UpdateActions()
        {
            Actions_CollectionChanged(null,null);
            ((INotifyCollectionChanged)Model.Actions).CollectionChanged += Actions_CollectionChanged;
        }

        private readonly object _lock = new object();
        private void Actions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
            () =>
                {
                    lock(_lock)
                    {
                        _actions.Clear();
                        _highlights.Clear();
                        _backwardActions.Clear();
                        foreach (var m in Model.Actions)
                        {
                            switch (m.Direction)
                            {
                                case WorkflowDirection.Forward:
                                    _actions.Add(m);
                                    foreach (var item in Model.Highlights)
                                    {
                                        _highlights.Add(item);
                                    }
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