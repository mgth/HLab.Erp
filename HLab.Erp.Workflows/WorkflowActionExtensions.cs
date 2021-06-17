using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        /// <param name="c"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static TC VisibleWhen<TC, TWf>(this TC c, Func<TWf, bool> condition)
            where TWf : class, IWorkflow<TWf>
             where TC : IFluentConfigurator<IWorkflowConditionalObject<TWf>>
        {
            c.Target.AddCondition(new WorkflowCondition<TWf>(w =>
                condition(w) ? WorkflowConditionResult.Passed : WorkflowConditionResult.Hidden));
            return c;
        }

        /// <summary>
        /// Add condition to prevent action
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            NotWhen<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, Func<TWf, bool> condition)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            c?.Target.AddCondition(new WorkflowCondition<TWf>(w =>
                condition(w) ? WorkflowConditionResult.Failed : WorkflowConditionResult.Passed));
            return c;
        }

        /// <summary>
        /// Add condition to allow action
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            When<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, Func<TWf, bool> condition)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            c?.Target.AddCondition(new WorkflowCondition<TWf>(w =>
                condition(w) ? WorkflowConditionResult.Passed : WorkflowConditionResult.Failed));
            return c;
        }

        /// <summary>
        /// Add message factory to inform about why the action is not possible
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            WithMessage<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, Func<TWf, string> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            c?.Target.Condition?.SetMessage(w => new List<string> { getter(w) });
            return c;
        }

        /// <summary>
        /// Add lambda to field to highlight when action is not allowed
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            HighlightField<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, Expression<Func<TWf, object>> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            Expression body = getter.Body;
            if (body.NodeType == ExpressionType.Convert)
                body = ((UnaryExpression)body).Operand;

            var name = body.ToString().Split('.').Last();
            c?.Target.Condition?.SetHighlights(w => new List<string> { name });
            return c;
        }

        /// <summary>
        /// Add message to inform about why the action is not possible
        /// </summary>
        /// <typeparam name="TC"></typeparam>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            WithMessage<TC, TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, string message)
            where TWf : NotifierBase, IWorkflow<TWf>
            => c.WithMessage(e => message);

        /// <summary>
        /// Add caption to a workflow element
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            Caption<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, Func<TWf, string> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
            //            where T : IWorkflowConditionalObject<TWf>
            //            where TC : IFluentConfigurator<T>
        {
            c?.Target.SetCaption(getter);
            return c;
        }

        /// <summary>
        /// Add caption factory to a workflow element
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            Caption<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, string caption)
            where TWf : NotifierBase, IWorkflow<TWf>
            //            where TC : IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            => c.Caption<TWf>(e => caption);

        /// <summary>
        /// Add Icon factory to a workflow element
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            Icon<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, Func<TWf, string> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            c?.Target.SetIconPath(getter);
            return c;
        }

        /// <summary>
        /// Add progress calculator to workflow element
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            Progress<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, Func<TWf, double> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            c?.Target.SetProgress(getter);
            return c;
        }

        /// <summary>
        /// Add Icon factory to a workflow element
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            SubIcon<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, Func<TWf, string> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            c?.Target.SetSubIconPath(getter);
            return c;
        }

        /// <summary>
        /// Add Icon path to a workflow element.
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            Icon<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, string icon)
            where TWf : NotifierBase, IWorkflow<TWf>
            => c.Icon<TWf>(e => icon);

        /// <summary>
        /// Define workflow progress value after action
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            Progress<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, double progress)
            where TWf : NotifierBase, IWorkflow<TWf>
            => c.Progress<TWf>(e => progress);

        /// <summary>
        /// Add Icon path to a workflow element.
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            SubIcon<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, string icon)
            where TWf : NotifierBase, IWorkflow<TWf>
            => c.SubIcon<TWf>(e => icon);

        /// <summary>
        /// Enable an action when some state is allowed
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="getStage"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>>
            WhenStageAllowed<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, Func<Workflow<TWf>.Stage> getStage)
            where TWf : class, IWorkflow<TWf>
        {
            var cond = new WorkflowCondition<TWf>(w =>
                getStage().Check(w) == WorkflowConditionResult.Passed
                    ? WorkflowConditionResult.Passed
                    : WorkflowConditionResult.Failed);

            cond.SetMessage(w => getStage().GetMessages(w));
            cond.SetHighlights(w => getStage().GetHighlights(w));

            c?.Target.AddCondition(cond);
            return c;
        }

        /// <summary>
        /// Enable an action when current stage is in provided list
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="getters">List of lambdas pointing to allowed stages</param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> FromStage<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, params Func<Workflow<TWf>.Stage>[] getters)
            where TWf : class, IWorkflow<TWf>
        {
            c?.Target.AddCondition(new WorkflowCondition<TWf>(w =>
            {
                foreach (var getState in getters)
                    if (w.CurrentStage == getState()) return WorkflowConditionResult.Passed;

                return WorkflowConditionResult.Hidden;
            }));
            return c;
        }

        /// <summary>
        /// Define action for a workflow element
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> Action<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, Action<TWf> action)
            where TWf : class, IWorkflow<TWf>
        {
            c?.Target.SetAction(action);
            return c;
        }

        /// <summary>
        /// Define target stage for a workflow element
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="getStage"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> ToStage<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, Func<Workflow<TWf>.Stage> getStage)
            where TWf : class, IWorkflow<TWf>
        {
            if (c.Target is Workflow<TWf>.Action action)
            {
                c.WhenStageAllowed(getStage);
                c.Action(async w => await w.SetStageAsync(getStage, action.GetCaption(w), action.GetIconPath(w), action.SigningMandatory, action.MotivationMandatory));
                return c;
            }
            return c;
        }

        /// <summary>
        /// Set the action to be backward action
        /// </summary>
        /// <typeparam name="TC"></typeparam>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> Backward<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c)
            where TWf : class, IWorkflow<TWf>
        {
            if (c.Target is Workflow<TWf>.Action action)
                action.Direction = WorkflowDirection.Backward;
            return c;
        }

        /// <summary>
        /// Specify that action requires signature
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> Sign<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c)
            where TWf : class, IWorkflow<TWf>
        {
            if (c.Target is Workflow<TWf>.Action action)
                action.SigningMandatory = true;
            return c;
        }

        /// <summary>
        /// Specify that action requires motivation 
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> Motivate<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c)
            where TWf : class, IWorkflow<TWf>
        {
            if (c.Target is Workflow<TWf>.Action action)
                action.MotivationMandatory = true;
            return c;
        }
    }
}
