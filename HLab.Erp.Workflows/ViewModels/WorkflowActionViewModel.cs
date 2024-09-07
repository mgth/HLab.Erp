﻿using System.Windows.Input;
using HLab.Erp.Workflows.Models;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;

namespace HLab.Erp.Workflows.ViewModels;

public class WorkflowActionViewModel : ViewModel<WorkflowAction>
{
    public WorkflowActionViewModel()
    {
        _iconPath = this.WhenAnyValue(vm => vm.Model.IconPath)
            .ToProperty(this, vm => vm.IconPath);

        Command = ReactiveCommand.Create(
            () => Model.Action(),
            this.WhenAnyValue(
                vm => vm.Model,
                wf => wf.Check() == WorkflowConditionResult.Passed
                ));
    }

    public string IconPath => _iconPath.Value;
    readonly ObservableAsPropertyHelper<string> _iconPath;

    public ICommand Command { get; }
}