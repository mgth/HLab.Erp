using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using HLab.Base.Fluent;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{
    public class WorkflowState<T> : WorkflowConditionalObject<T> 
        where T : NotifierBase, IWorkflow<T>
    {
        public static WorkflowState<T> Create(
            Action<IFluentConfigurator<WorkflowState<T>>> configure, 
            [CallerMemberName]string name="")
        {
            var state =  new WorkflowState<T>(name);
            var configurator = new FluentConfigurator<WorkflowState<T>>(state);
            configure(configurator);
            Workflow<T>.AddState(state);
            return state;
        }

        public string Name { get; }

        private WorkflowState(string name)
        {
            Name = name;
        }
    }

}
