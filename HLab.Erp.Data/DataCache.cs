using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using HLab.DependencyInjection;
using NPoco;

namespace HLab.Erp.Data
{
    public interface IDataProvider
    {
        IDataService DataService { get; set; }
    }

    internal class DataCache
    {
        public DataService DataService { get; set; }
        public DependencyInjectionContainer Container { get; set; }
    }

    internal class DataCache<T> : DataCache where T : class, IEntity
    {
        private readonly ConcurrentDictionary<object,T> _cache = new ConcurrentDictionary<object,T>();
        private bool _fullCache = false;
        public List<T> Fetch(Expression<Func<T, bool>> expression)
        {
            var e = expression.Compile();

            if (!_fullCache)
            {
                var list = new List<T>();
                foreach (var obj in DataService.Get().Fetch<T>())
                {
                    var cached = GetOrAdd(obj);
                    if(e(cached)) list.Add(cached);
                }

                _fullCache = true;
                return list;
            }
            return _cache.Values.Where(e).ToList();
        }

        public T Get(object key)
        {
            var subscribe = false;

            var obj = _cache.GetOrAdd(key,
                k =>
                {
                    subscribe = true;
                    var o = DataService.Get().SingleById<T>(key);
                    return o;
                });

                //if(subscribe && obj is INotifierObject nobj) nobj.GetNotifier().Subscribe();
            if (subscribe && obj is IEntity entity) entity.OnLoaded();
            if (subscribe && obj is IDataProvider dbf) dbf.DataService = DataService;

            return obj;
        }

        public void Forget(T obj)
        {
            if (_cache.TryRemove(obj.Id, out var old))
            {
            }
        }

        public T GetOrAdd(T obj)
        {
            var result = _cache.GetOrAdd(obj.Id,
                k => obj);

                if (result != null && !ReferenceEquals(result,obj))
                {
                    //foreach (var info in result.GetType().GetProperties().Where(p => p.GetCustomAttributes(true).OfType<ColumnAttribute>().Any()))
                    foreach (var info in result.GetType().GetProperties().Where(p => p.CanWrite))
                    {
                        var t = info.PropertyType;
                        if (t.IsConstructedGenericType)
                        {
                            if (info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                t = t.GetGenericArguments()[0];
                            }
                        }

                        if(t.IsPrimitive
                        || t == typeof(string)
                        || t == typeof(DateTime)
                        || t == typeof(Byte[])
                        )
                            info.SetValue(result,info.GetValue(obj));
                        else
                        {
                            if (!typeof(IEntity).IsAssignableFrom(t))
                            {

                            }
                        }
                    }
            } 
            
            if (result is IDataProvider dbf && dbf.DataService==null) 
                dbf.DataService = DataService;

            return result;
        }
    }
}
