using System;
using System.Linq.Expressions;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Data;

namespace HLab.Erp.Workflows
{
    public static class WorkflowFilterExtensions
    {
        public static IColumnConfigurator<T, string, WorkflowFilter<TC>> Link<T, TC>(this IColumnConfigurator<T, string, WorkflowFilter<TC>> fc, Expression<Func<T, string>> getter) 
            where T : class, IEntity, new()
            where TC : class, IWorkflow<TC>
        {
            fc.Filter.Link(fc.Target.List, getter);
            return fc;
        }
    }
}