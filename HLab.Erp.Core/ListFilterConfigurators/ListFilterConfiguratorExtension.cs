using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using TextFilter = HLab.Erp.Core.ListFilters.TextFilter;

namespace HLab.Erp.Core.ListFilterConfigurators
{
    public static class ListFilterConfiguratorExtension
    {
        /// <summary>
        /// Start un new column
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TLink"></typeparam>
        /// <typeparam name="TFilter"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static IColumnConfigurator<T,object,IFilter<object>> Column<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            var result = new ColumnConfigurator<T,object,IFilter<object>>(c.Target);
            c.Dispose();
            return result;
        }

        /// <summary>
        /// Define a static filter that will not be editable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TLink"></typeparam>
        /// <typeparam name="TFilter"></typeparam>
        /// <param name="c"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IColumnConfigurator<T, TLink, TFilter> StaticFilter<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, bool>> filter)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            c.Target.List.AddFilter(filter);
            return c;
        }
        //public static IColumnConfigurator<T, TLinkOut, IFilter<TLinkOut>> Link<T, TLink, TLinkOut, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, TLinkOut>> getter)
        //    where T : class, IEntity, new()
        //{
        //    var result = new ColumnConfigurator<T, TLinkOut, IFilter<TLinkOut>>(c.Helper, getter)
        //    {
        //        //CurrentFilter = new TextFilter()
        //    };
        //    result.CurrentFilter.Link(c.Helper.Target.List, result.Getter);

        //    return result;
        //}

        public static IColumnConfigurator<T, string, IFilter<string>> Link<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, string>> getter)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            var result = c.GetChildConfigurator<string, IFilter<string>>();
            result.Link = getter;

            return result;
        }

        public static IColumnConfigurator<T, DateTime?, IFilter<DateTime?>> Link<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, DateTime?>> getter)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            var result = c.GetChildConfigurator<DateTime?, IFilter<DateTime?>>();
            result.Link = getter;

            return result;
        }

        //1
        public static IColumnConfigurator<T, TLinkOut, IFilter<TLinkOut>> LinkGeneric<T, TLink, TFilter, TLinkOut>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, TLinkOut>> getter)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            var result = c.GetChildConfigurator<TLinkOut, IFilter<TLinkOut>>();
            result.Link = getter;

            return result;
        }

        //2
        public static IColumnConfigurator<T, int, EntityFilter<TE>> Link<T, TE, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, TE>> getter)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        where TE : class, IEntity<int>, new()
        {
            var result = c.GetChildConfigurator<int, EntityFilter<TE>>();
            result.Link = GetterIdFromGetter(getter);

            return result;
        }

        public static IColumnConfigurator<T, int?, EntityFilterNullable<TE>> LinkNullable<T, TE, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, TE>> getter)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        where TE : class, IEntity<int>, new()
        {
            var result =
                c.GetChildConfigurator<int?, EntityFilterNullable<TE>>();

            result.Link = GetterIdNullableFromGetter(getter);

            return result;
        }

        public static IColumnConfigurator<T, TLink, TFilterOut> Filter<T, TLink, TFilterIn, TFilterOut>(this IColumnConfigurator<T, TLink, TFilterIn> c, TFilterOut filter)
            where T : class, IEntity, new()
            where TFilterOut : IFilter<TLink>
            where TFilterIn : IFilter<TLink>
        {
            var result = c.GetChildConfigurator<TLink, TFilterOut>();
            
            result.Filter
                ?.Link(c.Target.List, c.Link);

            return result;
        }

        public static IColumnConfigurator<T, string, TextFilter> Filter<T, TFilter>(this IColumnConfigurator<T, string, TFilter> c)
            where T : class, IEntity, new()
            where TFilter : IFilter<string>
        {
            var result = c.GetChildConfigurator<string, TextFilter>();

            result.Filter
                ?.Link(c.Target.List, c.Link);

            return result;
        }

        public static IColumnConfigurator<T, DateTime?, DateFilterNullable> Filter<T, TFilter>(this IColumnConfigurator<T, DateTime?, TFilter> c)
            where T : class, IEntity, new()
            where TFilter : IFilter<DateTime?>
        {
            var result = c.GetChildConfigurator<DateTime?, DateFilterNullable>();

            result.Filter
                .Link(c.Target.List, c.Link);

            return result;
        }
        public static IColumnConfigurator<T, DateTime, DateFilter> Filter<T, TFilter>(this IColumnConfigurator<T, DateTime, TFilter> c)
            where T : class, IEntity, new()
            where TFilter : IFilter<DateTime>
        {
            return c.GetChildConfigurator<DateTime, DateFilter>();
        }

        private static readonly MethodInfo GetIdMethod = typeof(IEntity<int>).GetProperty("Id").GetMethod;

        private static Expression<Func<T, int>> GetIdExpression<T, TE>(Expression<Func<T, TE>> getter)
            where T : class, IEntity, new()
            where TE : class, IListableModel, IEntity<int>, new()
        {
            var entity = getter.Parameters[0];
            var ex = Expression.Call(getter.Body, GetIdMethod);
            return Expression.Lambda<Func<T, int>>(ex, entity);
        }

        private static PropertyInfo PropertyHasValue => typeof(int?).GetProperty("HasValue");
        private static PropertyInfo PropertyValue => typeof(int?).GetProperty("Value");

        // TODO : may return int?
        private static Expression<Func<T, int>> GetterIdFromGetter<T, TE>(Expression<Func<T, TE>> getter)
            where T : class, IEntity, new()
            where TE : class, IEntity<int>, new()
        {
            if (getter.Body is MemberExpression member)
            {
                var name = member.Member.Name;
                var method = typeof(T).GetProperty($"{name}Id");//?.GetMethod;

                var entity = Expression.Parameter(typeof(T), "e");
                if (method != null)
                {
                    var property = Expression.Property(entity, method);

                    if (method.PropertyType.IsGenericType &&
                        method.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return Expression.Lambda<Func<T, int>>(
                        Expression.Condition(
                            Expression.Call(property, PropertyHasValue.GetMethod),
                            Expression.Call(property, PropertyValue.GetMethod),
                            Expression.Constant(-1,typeof(int))), entity);
                    }

                    return Expression.Lambda<Func<T, int>>(property, entity);
                }
            }

            return t => -1;
        }

        private static Expression<Func<T, int?>> GetterIdNullableFromGetter<T, TE>(Expression<Func<T, TE>> getter)
            where T : class, IEntity, new()
            where TE : class, IEntity<int>, new()
        {
            if (getter.Body is MemberExpression member)
            {
                var name = member.Member.Name;
                var method = typeof(T).GetProperty($"{name}Id")?.GetMethod;

                if (method?.ReturnType != typeof(int?)) throw new InvalidOperationException("Entity should have been nullable");

                var entity = Expression.Parameter(typeof(T), "e");
                var property = Expression.Property(entity, method);
                return Expression.Lambda<Func<T, int?>>(property, entity);
            }

            return t => -1;
        }


        public static IColumnConfigurator<T, int, EntityFilter<TE>> Filter<T, TE>(this IColumnConfigurator<T, int, EntityFilter<TE>> c)
            where T : class, IEntity, new()
            where TE : class, IEntity<int>, new()

        {
            //var getter = GetterIdFromGetter(c.Getter);

            return  c.GetChildConfigurator<int, EntityFilter<TE>>();
        }

        public static IColumnConfigurator<T, int?, EntityFilterNullable<TE>> Filter<T, TE>(this IColumnConfigurator<T, int?, EntityFilterNullable<TE>> c)
            where T : class, IEntity, new()
            where TE : class, IEntity<int>, new()

        {
            //var getter = GetterIdFromGetter(c.Getter);

            return c.GetChildConfigurator<int?, EntityFilterNullable<TE>>();
        }
        public static IColumnConfigurator<T, int?, EntityFilterNullable<TE>> Column<T, TLink, TFilter, TE>(this IColumnConfigurator<T, TLink, TFilter> c,
            Expression<Func<T, TE>> getter,
            Expression<Func<T, int?>> getterId = null,
            double width = double.NaN
        )
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
            where TE : class, IListableModel, IEntity<int>, new()
        {
            var lambda = getter.Compile();
            getterId ??= GetterIdNullableFromGetter(getter);

            return c.Column()
                .Header($"{{{typeof(TE).Name}}}")
                .Width(width)
                .OrderBy(e => lambda(e)?.Caption)
                .LinkNullable(getter)
                // TODO                .Icon(e => lambda(e)?.IconPath)
                .Filter()
                ;
        }


        public static IColumnConfigurator<T, TLink, TFilter> Header<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, object caption)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            c.Column.Header = caption;
            return c;
        }

        public static IColumnConfigurator<T, TLink, TFilter> Width<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, double width)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            c.Column.Width = width;
            return c;
        }
        public static IColumnConfigurator<T, TLink, TFilter> OrderBy<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Func<T, object> orderBy)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            c.Column.OrderBy = orderBy;
            return c;
        }

        public static IColumnConfigurator<T, string, TextFilter> PostLink<T>(this IColumnConfigurator<T, string, TextFilter> tf, Func<T, string> getter)
            where T : class, IEntity, new()
        {
            tf.Filter.PostLink(tf.Target.List, getter);
            return tf;
        }
        public static IColumnConfigurator<T, string, TextFilter> Link<T>(this IColumnConfigurator<T, string, TextFilter> tf, Expression<Func<T, string>> getter)
            where T : class, IEntity, new()
        {
            tf.Link = getter;
            tf.Filter.Link(tf.Target.List, getter);
            return tf;
        }
        //public static IColumnConfigurator<T, TLink, IFilter<TLink>> Link<T, TLink>(this IColumnConfigurator<T, TLink, IFilter<TLink>> tf, Expression<Func<T, TLink>> getter)
        //    where T : class, IEntity, new()
        //{
        //    tf.CurrentFilter.Link(tf.Helper.Target.List, getter);
        //    return tf;
        //}

        public static T Id<T>(this T c, string id)
            where T : IColumnConfigurator
        {
            c.Column.Id = id;
            return c;
        }

        public static T Hidden<T>(this T c)
            where T : IColumnConfigurator
        {
            c.Column.Hidden = true;
            return c;
        }
        public static IColumnConfigurator<T, TLink, TFilter> IconPath<T,TLink,TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, string path)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            if (string.IsNullOrWhiteSpace(c.Column.IconPath)) c.Column.IconPath = path;

            var filter = c.Filter;

            if (filter != null) 
                filter.IconPath = path;

            return c;

        }

    }
}
