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

        public async Task<T> GetOrAdd(object key, Func<object, Task<T>> factory) => await _cache.GetOrAdd(key, factory).ConfigureAwait(false);

        public async Task<bool> Forget(T obj)
        {
            var r = await _cache.TryRemove(obj.Id).ConfigureAwait(false);
            return r.Item1;
        }

        public async Task<List<T>> GetOrAdd(List<T> list)
        {
            var listOut = new List<T>();

            await Task.Run(()=>list.ForEach(async e =>
            {
                var obj = await GetOrAdd(e).ConfigureAwait(true);
                listOut.Add(obj);
            })).ConfigureAwait(false);
            return listOut;
        }

        public async Task<T> GetOrAdd(T obj)
        {
            var result = await _cache.GetOrAdd(obj.Id,
                async k => obj).ConfigureAwait(false);

            obj.CopyPrimitivesTo(result);

            if (result is IDataProvider dbf && dbf.DataService==null) 
                dbf.DataService = DataService;

            return result;
        }
    }
}
