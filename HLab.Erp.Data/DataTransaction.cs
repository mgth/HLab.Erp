using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NPoco;

namespace HLab.Erp.Data
{
    public class DataTransaction : IDataTransaction
    {
        private readonly DataService _service;
        private readonly ITransaction _transaction;
        internal IDatabase Database;
        private Action _rollback = default(Action);

        public DataTransaction(DataService service, IDatabase database)
        {
            _service = service;
            Database = database;

            try
            {
                _transaction = database.GetTransaction();

            }
            catch (Exception ex)
            {
                throw new DataException("Failed to create new transaction", ex);
            }
        }

        public T Add<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity
        {
            var t = (T)Activator.CreateInstance(typeof(T));
            if (t == null) throw new NullReferenceException();

            setter?.Invoke(t);

            object e = null;
            if (typeof(T).GetCustomAttributes<SoftIncrementAttribut>().Any())
            {
                if (t is IEntity<int> ti)
                {
                    var ids = Database.Query<T>().OrderByDescending(d => ((IEntity<int>)d).Id).FirstOrDefault();

                    var id = ((IEntity<int>)ids)?.Id ?? 0;

                    id++;

                    ti.Id = id;
                }
            }

            Database.Insert(t);

            t.IsLoaded = true;
            added?.Invoke(t);

            return DataCache<T>.Cache.GetOrAddAsync(t).Result;
        }

        public async Task<T> AddAsync<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity
        {
            var t = (T)Activator.CreateInstance(typeof(T));  //_entityFactory(typeof(T));
            if (t == null) throw new NullReferenceException();
            
            if(t is IEntity<int> tt) tt.Id=-1;

            setter?.Invoke(t);

            object e = null;
            if (typeof(T).GetCustomAttributes<SoftIncrementAttribut>().FirstOrDefault() is SoftIncrementAttribut a)
            {
                if (t is IEntity<int> ti)
                {
                    var ids = await Database.QueryAsync<T>().OrderByDescending(d => ((IEntity<int>) d).Id)
                        .FirstOrDefault().ConfigureAwait(false);

                    var id = ((IEntity<int>) ids)?.Id ?? 0;

                    id++;

                    ti.Id = id;

                }
            }

            e = await Database.InsertAsync(t).ConfigureAwait(false);

            if (e != null)
            {
                t.IsLoaded = true;
                added?.Invoke(t);
            }

            return await DataCache<T>.Cache.GetOrAddAsync(t).ConfigureAwait(false);
        }

        public void Update<T>(T value, IEnumerable<string> columns) where T : class, IEntity
        {
            Database.Update(value, columns);
        }

        public async Task<bool> UpdateAsync<T>(T value, IEnumerable<string> columns) where T : class, IEntity
        {
            var n = await Database.UpdateAsync(value, columns);
            return n > 0;
        }

        public void Save<T>(T value) where T : class, IEntity
        {
            Database.Save(value);
        }

        public Task SaveAsync<T>(T value) where T : class, IEntity
        {
            return Task.Run(() => Database.Save(value));
        }

        public bool Update<T>(T value, params string[] columns) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync<T>(T value, params string[] columns) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public bool Update<T>(T value, Action<T> setter) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync<T>(T value, Action<T> setter) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public bool Delete<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity
        {
            var result = Database.Delete<T>(entity);
            if (result <= 0) return false;
            
            _ = DataCache<T>.Cache.ForgetAsync(entity);
            deleted?.Invoke((T)entity);
            
            return true;
        }

        public async Task<bool> DeleteAsync<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity
        {
            var result = await Database.DeleteAsync(entity);
            if (result <= 0) return false;
            
            await DataCache<T>.Cache.ForgetAsync(entity);
            deleted?.Invoke((T)entity);
            
            return true;
        }

        public void Done()
        {
            _transaction.Complete();
        }

        public void ExecuteSql(string sql)
        {
            Database.Execute(sql);
        }

        public void Dispose()
        {
            try
            {
                _transaction.Dispose();

            }
            catch (Exception ex)
            {
                throw new DataException("Failed to close transaction",ex);
            }
        }
    }
}