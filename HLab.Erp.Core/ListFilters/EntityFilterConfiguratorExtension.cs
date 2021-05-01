using System;
using System.Linq.Expressions;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ListFilters
{
    public static class EntityFilterConfiguratorExtension
    {
        public static IFilterConfigurator<T, IEntityFilterNotNull<TE>> Link<T, TE>(this IFilterConfigurator<T, IEntityFilterNotNull<TE>> fc, Expression<Func<T, int>> getter) 
            where T : class, IEntity, new()
            where TE : class, IEntity, IListableModel, new()
        {
            fc.CurrentFilter.Link(fc.Target().List, getter);
            return fc;
        }

        public static IFilterConfigurator<T, IEntityFilterNullable<TE>> Link<T, TE>(this IFilterConfigurator<T, IEntityFilterNullable<TE>> fc, Expression<Func<T, int?>> getter) 
            where T : class, IEntity, new()
            where TE : class, IEntity, IListableModel, new()
        {
            fc.CurrentFilter.Link(fc.Target().List, getter);
            return fc;
        }

        public static IFilterConfigurator<T, IEntityFilterNotNull<TE>> PostLink<T, TE>(this IFilterConfigurator<T, IEntityFilterNotNull<TE>> fc, Func<T, int> getter) 
            where T : class, IEntity, new()
            where TE : class, IEntity, IListableModel, new()
        {
            fc.CurrentFilter.PostLink(fc.Target().List, getter);
            return fc;
        }

        public static IFilterConfigurator<T, IEntityFilterNullable<TE>> PostLink<T, TE>(this IFilterConfigurator<T, IEntityFilterNullable<TE>> fc, Func<T, int?> getter) 
            where T : class, IEntity, new()
            where TE : class, IEntity, IListableModel, new()
        {
            fc.CurrentFilter.PostLink(fc.Target().List, getter);
            return fc;
        }
    }
}