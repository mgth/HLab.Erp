using System;
using System.Linq.Expressions;

namespace HLab.Erp.Data.Observables
{
    public static class ObservableQueryExtensions
    {
        public static ObservableQuery<T> AddFilter<T>(this ObservableQuery<T> oq, object name, Func<Expression<Func<T, bool>>> expression, int order = 0)
            where T : class, IEntity
        {
            oq.AddFilter(expression, order, name);
            return oq;
        }
        public static ObservableQuery<T> AddFilter<T>(this ObservableQuery<T> oq, object name, Expression<Func<T, bool>> expression, int order = 0)
            where T : class, IEntity
        {
            oq.AddFilter(expression, order, name);
            return oq;
        }

        public static ObservableQuery<T> FluentUpdate<T>(this ObservableQuery<T> oq, bool force = true)
            where T : class, IEntity
        {
            oq.UpdateAsync(force);
            return oq;
        }
    }
}