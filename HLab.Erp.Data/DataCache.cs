using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HLab.Base;
using HLab.DependencyInjection;

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
                using var db = DataService.Get();
                var dbList = await db.FetchAsync<T>().ConfigureAwait(false);

                // TODO : bof
                foreach (var obj in dbList)
                {
                    var cached = await GetOrAdd(obj).ConfigureAwait(false);
                    if(e(cached)) list.Add(cached);
                }

                _fullCache = true;
                return list;
            }
            return await _cache.Where(expression).ConfigureAwait(false);
        }

        public async Task<T> GetOrAdd(object key, Func<object, Task<T>> factory) 
            => await _cache.GetOrAdd(key, factory).ConfigureAwait(false);

        public async Task<bool> Forget(T obj)
        {
            var r = await _cache.TryRemove(obj.Id).ConfigureAwait(false);
            return r.Item1;
        }

        public async Task<List<T>> GetOrAdd(List<T> list)
        {
            var listOut = new List<T>();
            foreach (var e in list)
            {
                var obj = await GetOrAdd(e).ConfigureAwait(true);
                listOut.Add(obj);
            }
            //await Task.Run(()=>
            //})).ConfigureAwait(true);
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
