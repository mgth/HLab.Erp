using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HLab.Base;

namespace HLab.Erp.Data
{
    public interface IDataServiceProvider
    {
        IDataService DataService { get; set; }
    }

    internal class DataCache
    {
        public static DataService DataService { get; set; }
    }

    internal class DataCache<T> : DataCache where T : class, IEntity
    {
        public static DataCache<T> Cache = new();

        readonly AsyncDictionary<object,T> _cache = new();
        bool _fullCache = false;

        public IEnumerable<T> Fetch(Expression<Func<T, bool>> expression)
        {
#if DEBUG
            var literal = expression.ToString();
#endif

            var e = expression.Compile();

            if (!_fullCache)
            {
                using var db = DataService.Get();
                var dbList = db.Query<T>().ToEnumerable();

                foreach (var obj in dbList)
                {
                    var cached = GetOrAdd(obj);
                    if(e(cached)) yield return cached;
                }

                _fullCache = true;
            }
            else
                foreach (var item in _cache.Where(expression)) yield return item;
        }

        public async IAsyncEnumerable<T> FetchAsync(Expression<Func<T, bool>> expression)
        {
            #if DEBUG
            var literal = expression.ToString();
            #endif

            var e = expression.Compile();

            if (!_fullCache)
            {
                using var db = DataService.Get();
                var dbList = db.QueryAsync<T>().ToEnumerable().ConfigureAwait(false);

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
        public IEnumerable<T> GetOrAdd(IEnumerable<T> list)
        {
            foreach (var e in list)
            {
                var obj = GetOrAdd(e);
                yield return obj;
            }
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
                 k => Task.FromResult(obj)).ConfigureAwait(true);

            if(!ReferenceEquals(obj,result))
                obj.CopyPrimitivesTo(result);

            if (result is IDataServiceProvider {DataService: null} dbf) 
                dbf.DataService = DataService;

            return result;
        }

        public T GetOrAdd(T obj)
        {
            var result = _cache.GetOrAdd(obj.Id, k => obj);

            if(!ReferenceEquals(obj,result))
                obj.CopyPrimitivesTo(result);

            if (result is IDataServiceProvider {DataService: null} dbf) 
                dbf.DataService = DataService;

            return result;
        }
    }
}
