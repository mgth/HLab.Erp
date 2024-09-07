using System;
using System.Linq.Expressions;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Data;
using HLab.Erp.Workflows.Interfaces;
using HLab.Erp.Workflows.Models;

namespace HLab.Erp.Workflows.Extensions;

public static class WorkflowFilterExtensions
{
    public static IColumnConfigurator<T, string, WorkflowFilter<TC>> Link<T, TC>(this IColumnConfigurator<T, string, WorkflowFilter<TC>> @this, Expression<Func<T, string>> getter)
        where T : class, IEntity, new()
        where TC : class, IWorkflow<TC>
    {
        return @this.FilterLink(getter);
    }
}