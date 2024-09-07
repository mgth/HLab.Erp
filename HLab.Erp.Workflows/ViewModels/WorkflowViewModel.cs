using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using HLab.Erp.Workflows.Interfaces;
using HLab.Erp.Workflows.Models;



//using System.Windows;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;

namespace HLab.Erp.Workflows.ViewModels;

public class WorkflowViewModel : ViewModel<IWorkflow>
{
    readonly ObservableCollection<WorkflowAction> _backwardActions = [];
    readonly ObservableCollection<WorkflowAction> _actions = [];
    readonly ObservableCollection<string> _highlights = [];
    public ReadOnlyObservableCollection<WorkflowAction> BackwardActions { get; }
    public ReadOnlyObservableCollection<WorkflowAction> Actions { get; }
    public ReadOnlyObservableCollection<string> Highlights { get; }

    public WorkflowViewModel()
    {
        BackwardActions = new(_backwardActions);
        Actions = new(_actions);
        Highlights = new(_highlights);

        this.WhenAnyValue(vm => vm.Model)
            .Subscribe(_ => UpdateActions());
    }

    public void UpdateActions()
    {
        Actions_CollectionChanged(null, null);
        ((INotifyCollectionChanged)Model.Actions).CollectionChanged += Actions_CollectionChanged;
    }

    readonly object _lock = new();

    void Actions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // TODO 
        //Application.Current.Dispatcher.Invoke(
        //() =>
        //    {
        lock (_lock)
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
        //    }
        //);
    }
}