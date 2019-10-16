using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using HLab.Base;
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
        private readonly AsyncDictionary<object,T> _cache = new AsyncDictionary<object,T>();
        private bool _fullCache = false;
        public async Task<List<T>> Fetch(Expression<Func<T, bool>> expression)
        {
            var e = expression.Compile();

            if (!_fullCache)
            {
                var list = new List<T>();

                var dbList = await DataService.Get().FetchAsync<T>();

                // TODO : bof
                foreach (var obj in dbList)
                {
                    var cached = await GetOrAdd(obj);
                    if(e(cached)) list.Add(cached);
                }

                _fullCache = true;
                return list;
            }
            return await _cache.Where(expression);
        }

        public async Task<T> GetOrAdd(object key, Func<object, Task<T>> factory) => await _cache.GetOrAdd(key, factory);

        public async Task<bool> Forget(T obj)
        {
            var r = await _cache.TryRemove(obj.Id);
            return r.Item1;
        }

        public async Task<List<T>> GetOrAdd(List<T> list)
        {
            var listOut = new List<T>();

            await Task.Run(()=>list.ForEach(e => listOut.Add(GetOrAdd(e).Result)));
            return listOut;
        }

        public async Task<T> GetOrAdd(T obj)
        {
            var result = await _cache.GetOrAdd(obj.Id,
                async k => obj);

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
