using System;
using System.Collections.Generic;
using HLab.Base.Fluent;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{
    public static class WorkflowActionExtensions
    {
        public static TC VisibleWhen<TC,TWf>(this TC t, Func<TWf,bool> condition) 
            where TWf : NotifierBase, IWorkflow<TWf>
             where TC : IFluentConfigurator<IWorkflowConditionalObject<TWf>>
        {
                t.Target.AddCondition(new WorkflowCondition<TWf>(w =>
                    condition(w) ? WorkflowConditionResult.Passed : WorkflowConditionResult.Hidden));
            return t;
        }
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> NotWhen<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<TWf, bool> condition)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            t.Target.AddCondition(new WorkflowCondition<TWf>(w =>
                condition(w) ? WorkflowConditionResult.Failed : WorkflowConditionResult.Passed));
            return t;
        }
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> WithMessage<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<TWf, string> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            t.Target.Condition?.SetMessage(w => new List<string>{getter(w)});
            return t;
        }

        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> WithMessage<TC, TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, string message)
            where TWf : NotifierBase, IWorkflow<TWf>
            => t.WithMessage(e => message);

        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> Caption<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<TWf, string> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
        //            where T : IWorkflowConditionalObject<TWf>
            //            where TC : IFluentConfigurator<T>
        {
            t.Target.SetCaption(getter);
            return t;
        }

        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> Caption<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, string caption)
            where TWf : NotifierBase, IWorkflow<TWf>
//            where TC : IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            => t.Caption<TWf>(e => caption);

        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> Icon<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<TWf, string> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            t.Target.SetIcon(getter);
            return t;
        }

        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> Icon<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, string icon)
            where TWf : NotifierBase, IWorkflow<TWf>
            => t.Icon<TWf>(e => icon);

        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> WhenStateAllowed<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<Workflow<TWf>.State> getState)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
                var cond = new WorkflowCondition<TWf>(w =>
                    getState().Check(w) == WorkflowConditionResult.Passed
                        ? WorkflowConditionResult.Passed
                        : WorkflowConditionResult.Failed);

                cond.SetMessage(w => getState().GetMessages(w));

                t.Target.AddCondition(cond);
            return t;
        }

        //public static TC FromState<TC, TWf>(this TC t, Func<Workflow<TWf>.State> getState)
        //    where TWf : NotifierBase, IWorkflow<TWf>
        //    where TC : IFluentConfigurator<IWorkflowConditionalObject<TWf>>
        //{
        //        t.Target.AddCondition(new WorkflowCondition<TWf>(w =>
        //        (w.CurrentState == getState()) ? WorkflowConditionResult.Passed : WorkflowConditionResult.Hidden));
        //    return t;
        //}
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> FromState<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<Workflow<TWf>.State> getState)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
                t.Target.AddCondition(new WorkflowCondition<TWf>(w =>
                (w.CurrentState == getState()) ? WorkflowConditionResult.Passed : WorkflowConditionResult.Hidden));
            return t;
        }

        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> Action<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Action<TWf> action)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            t.Target.SetAction(action);
            return t;
        }

        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> ToState<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<Workflow<TWf>.State> getState)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            t.WhenStateAllowed(getState);           
            t.Action(w => w.SetState(getState));
            return t;
        }
        public static TC Backward<TC, TWf>(this TC t)
            where TWf : Workflow<TWf>
            where TC : IFluentConfigurator<WorkflowConditionalObject<TWf>>
        {
            //t.Target.;
            return t;
        }
    }
}
