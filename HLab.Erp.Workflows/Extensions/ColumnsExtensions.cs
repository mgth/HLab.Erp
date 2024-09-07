using System;
using System.Linq.Expressions;
using System.Reflection;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Erp.Workflows.Interfaces;
using HLab.Erp.Workflows.Models;

namespace HLab.Erp.Workflows.Extensions;

public static class WorkflowColumnsExtensions
{
    public static IColumnConfigurator<T,string,WorkflowFilter<TW>> StageColumn<T,TLink,TFilter,TW>(this IColumnConfigurator<T,TLink,TFilter> c,TW workflow, Expression<Func<T, string>> stageNameExpression) where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
        where TW : class,IWorkflow<TW>
    {
        var stageFromNameMethod = typeof(TW).GetMethod("StageFromName", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy/*,new []{typeof(string) }*/);
        var stageName = stageNameExpression.Compile();
        Func<T,IWorkflowStage> stage = e => StageFromName(stageName(e));

        return c.Column("Stage")
            .Header("{Stage}")
            .Width(180)
            .Localize(s => stage(s).GetCaption(null))
            .Icon(s => stage(s).GetIconPath(null), 20)
            .OrderBy(s => stage(s).Name)
            .Link(stageNameExpression)
            .Filter(default(WorkflowFilter<TW>))
            .Header("{Stage}")
            .IconPath("Icons/Workflow");

        IWorkflowStage StageFromName(string name) => (IWorkflowStage)stageFromNameMethod.Invoke(null,new []{name });
    }
}