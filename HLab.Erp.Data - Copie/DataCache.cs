using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;

using HLab.DependencyInjection;
using NPoco;

namespace HLab.Erp.Data
{
    public class DataCache
    {
        public Func<IDatabase> DatabaseFactory { get; set; }
        public DependencyInjectionContainer Container { get; set; }
    }

    public class DataCache<T> : DataCache where T : class, IEntity
    {
        private readonly ConcurrentDictionary<object,T> _cache = new ConcurrentDictionary<object,T>();

        public T Get(object key)
        {
            var subscribe = false;

            var obj = _cache.GetOrAdd(key,
                k =>
                {
                    subscribe = true;
                    var o = DatabaseFactory().SingleById<T>(key);
                    return o;
                });

                //if(subscribe && obj is INotifierObject nobj) nobj.GetNotifier().Subscribe();
            if (subscribe && obj is IEntity entity) entity.OnLoaded();

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
            var subscribe = false;
            var result = _cache.GetOrAdd(obj.Id,
                k =>
                {
                    subscribe = true;
                    return obj;
                });

            //if (subscribe && result is INotifierObject nobj)
            //{
            //    nobj.GetNotifier().Subscribe();
            //    result.OnLoaded();
            //}
            //else
            //{
                if (result != null)
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
//                }
            }
            return result;
        }
    }
}
