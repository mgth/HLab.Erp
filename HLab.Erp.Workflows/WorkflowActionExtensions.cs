using System;
using System.Collections.Generic;
using HLab.Base.Fluent;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{
    public static class WorkflowActionExtensions
    {
        /// <summary>
        /// Add condition to show action
        /// </summary>
        /// <typeparam name="TC"></typeparam>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static TC VisibleWhen<TC,TWf>(this TC t, Func<TWf,bool> condition) 
            where TWf : class,IWorkflow<TWf>
             where TC : IFluentConfigurator<IWorkflowConditionalObject<TWf>>
        {
                t.Target.AddCondition(new WorkflowCondition<TWf>(w =>
                    condition(w) ? WorkflowConditionResult.Passed : WorkflowConditionResult.Hidden));
            return t;
        }

        /// <summary>
        /// Add condition to prevent action
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> 
            NotWhen<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<TWf, bool> condition)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            t?.Target.AddCondition(new WorkflowCondition<TWf>(w =>
                condition(w) ? WorkflowConditionResult.Failed : WorkflowConditionResult.Passed));
            return t;
        }

        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> 
            When<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<TWf, bool> condition)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            t?.Target.AddCondition(new WorkflowCondition<TWf>(w =>
                condition(w) ? WorkflowConditionResult.Passed : WorkflowConditionResult.Failed));
            return t;
        }

        /// <summary>
        /// Add message factory to inform about why the action is not possible
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> 
            WithMessage<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<TWf, string> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            t?.Target.Condition?.SetMessage(w => new List<string>{getter(w)});
            return t;
        }

        /// <summary>
        /// Add message to inform about why the action is not possible
        /// </summary>
        /// <typeparam name="TC"></typeparam>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> 
            WithMessage<TC, TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, string message)
            where TWf : NotifierBase, IWorkflow<TWf>
            => t.WithMessage(e => message);

        /// <summary>
        /// Add caption to a workflow element
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> 
            Caption<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<TWf, string> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
        //            where T : IWorkflowConditionalObject<TWf>
            //            where TC : IFluentConfigurator<T>
        {
            t?.Target.SetCaption(getter);
            return t;
        }

        /// <summary>
        /// Add caption factory to a workflow element
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> 
            Caption<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, string caption)
            where TWf : NotifierBase, IWorkflow<TWf>
//            where TC : IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            => t.Caption<TWf>(e => caption);

        /// <summary>
        /// Add Icon factory to a workflow element
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> 
            Icon<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<TWf, string> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            t?.Target.SetIconPath(getter);
            return t;
        }

        /// <summary>
        /// Add Icon factory to a workflow element
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> 
            SubIcon<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<TWf, string> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            t?.Target.SetSubIconPath(getter);
            return t;
        }

        /// <summary>
        /// Add Icon path to a workflow element.
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> 
            Icon<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, string icon)
            where TWf : NotifierBase, IWorkflow<TWf>
            => t.Icon<TWf>(e => icon);

        /// <summary>
        /// Add Icon path to a workflow element.
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> 
            SubIcon<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, string icon)
            where TWf : NotifierBase, IWorkflow<TWf>
            => t.SubIcon<TWf>(e => icon);

        /// <summary>
        /// Enable an action when some state is allowed
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="getState"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> 
            WhenStateAllowed<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<Workflow<TWf>.State> getState)
            where TWf : class,IWorkflow<TWf>
        {
                var cond = new WorkflowCondition<TWf>(w =>
                    getState().Check(w) == WorkflowConditionResult.Passed
                        ? WorkflowConditionResult.Passed
                        : WorkflowConditionResult.Failed);

                cond.SetMessage(w => getState().GetMessages(w));

                t?.Target.AddCondition(cond);
            return t;
        }

        /// <summary>
        /// Enable an action when current state
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="getState"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> FromState<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<Workflow<TWf>.State> getState)
            where TWf : class,IWorkflow<TWf>
        {
                t?.Target.AddCondition(new WorkflowCondition<TWf>(w =>
                (w.CurrentState == getState()) ? WorkflowConditionResult.Passed : WorkflowConditionResult.Hidden));
            return t;
        }
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> FromStates<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, params Func<Workflow<TWf>.State>[] getters)
            where TWf : class,IWorkflow<TWf>
        {
                t?.Target.AddCondition(new WorkflowCondition<TWf>(w => {
                    foreach (var getState in getters)
                        if (w.CurrentState == getState()) return WorkflowConditionResult.Passed;
                    
                    return WorkflowConditionResult.Hidden;
                }));
            return t;
        }

        /// <summary>
        /// Define action for a workflow element
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> Action<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Action<TWf> action)
            where TWf : class,IWorkflow<TWf>
        {
            t?.Target.SetAction(action);
            return t;
        }

        /// <summary>
        /// Define target state for a workflow element
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="getState"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> ToState<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<Workflow<TWf>.State> getState)
            where TWf : class,IWorkflow<TWf>
        {
            t.WhenStateAllowed(getState);           
            t.Action(w => w.SetState(getState));
            return t;
        }

        /// <summary>
        /// Set the action to be backward action
        /// </summary>
        /// <typeparam name="TC"></typeparam>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> Backward<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t)
            where TWf : class, IWorkflow<TWf>
        {
            if(t.Target is Workflow<TWf>.Action action)
                action.Direction = WorkflowDirection.Backward;
            return t;
        }
    }
}
