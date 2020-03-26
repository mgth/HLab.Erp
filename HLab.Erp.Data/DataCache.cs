using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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


        public async IAsyncEnumerable<T> FetchAsync(Expression<Func<T, bool>> expression)
        {
            var e = expression.Compile();

            if (!_fullCache)
            {
                using var db = DataService.Get();
                var dbList = db.FetchAsync<T>().ConfigureAwait(false);

                // TODO : bof
                await foreach (var obj in dbList)
                {
                    var cached = await GetOrAddAsync(obj).ConfigureAwait(false);
                    if(e(cached)) yield return cached;
                }

                _fullCache = true;
            }
            else
                await foreach (var item in _cache.WhereAsync(expression)) yield return item;
        }

        public Task<T> GetOrAddAsync(object key, Func<object, Task<T>> factory) 
            => _cache.GetOrAddAsync(key, factory);

        public async Task<bool> ForgetAsync(T obj)
        {
            var r = await _cache.TryRemoveAsync(obj.Id).ConfigureAwait(false);
            return r.Item1;
        }

        public async IAsyncEnumerable<T> GetOrAddAsync(IAsyncEnumerable<T> list)
        {
            await foreach (var e in list)
            {
                var obj = await GetOrAddAsync(e).ConfigureAwait(true);
                yield return obj;
            }
        }

        public async Task<T> GetOrAddAsync(T obj)
        {
            var result = await _cache.GetOrAddAsync(obj.Id,
                async k => obj).ConfigureAwait(true);

            obj.CopyPrimitivesTo(result);

            if (result is IDataProvider dbf && dbf.DataService==null) 
                dbf.DataService = DataService;

            return result;
        }
        public T GetOrAdd(T obj)
        {
            var result = _cache.GetOrAdd(obj.Id, k => obj);

            obj.CopyPrimitivesTo(result);

            if (result is IDataProvider dbf && dbf.DataService==null) 
                dbf.DataService = DataService;

            return result;
        }
    }
}
