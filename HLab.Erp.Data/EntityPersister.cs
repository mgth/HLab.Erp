using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HLab.Base;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Data
{
    public class EntityPersister<T> : Persister
    where T : class, IEntity
    {
        private readonly IDataService _data;

        public EntityPersister(IDataService data, object target) : base(target,false)
        {
            _data = data;
        }

        protected new T Target => (T)base.Target;

        protected override Persistency CheckPersistency(PropertyInfo property)
        {
            if (!property.CanWrite) return Persistency.None;

            return property.GetCustomAttributes().OfType<IgnoreAttribute>().Any() ? Persistency.None : Persistency.OnSave;
        }
        public override bool Save()
        {
            var columns = new List<PropertyInfo>();
            while (Dirty.TryTake(out var e))
            {
                columns.Add(e);
            }

            try
            {
                if(Target is IEntity<int> ei && ei.Id<0)
                {
                    var t = _data.Add<T>(e => Target.CopyPrimitivesTo(e));
                    ei.Id = (int)t.Id;
                    return true;
                }
                if(_data.Update(Target, columns.Select(e => e.Name).ToArray()))
                {
                    IsDirty = false;
                    return true;
                }
                return false;
            }
            catch
            {
                foreach(var p in columns)
                    Dirty.Add(p);
                throw;
            }
        }

        private static string GetColumnName(PropertyInfo info)
        {
            var attr = info.GetCustomAttribute<ColumnAttribute>();
            if(attr?.Name != null) return attr.Name;
            return info.Name;
        }


        public override Task<bool> SaveAsync() => SaveAsync(null);
        public async Task<bool> SaveAsync(IDataTransaction transaction)
        {
            var tr = transaction ?? _data.GetTransaction();

            var columns = new List<PropertyInfo>();
            while (Dirty.TryTake(out var e))
            {
                columns.Add(e);
            }

            try
            {
                if (Target is IEntity<int> ei && ei.Id < 0)
                {
                    var t = await tr.AddAsync<T>(e => Target.CopyPrimitivesTo(e));
                    if (transaction == null) tr.Done();
                    ei.Id = (int) t.Id;
                    return true;
                }

                if (await tr.UpdateAsync(Target, columns.Select(GetColumnName)))
                {
                    if (transaction == null) tr.Done();

                    IsDirty = false;
                    return true;
                }

                return false;
            }
            catch
            {
                foreach (var p in columns)
                {
                    Dirty.Add(p);
                }

                throw;
            }
            finally
            {
                if (transaction == null) tr.Dispose();
            }
        }

        public override string ToString()
        {
            var columns = Dirty.ToList();
            var sb = new StringBuilder();
            foreach(var p in columns)
                sb
                    .Append(p.Name)
                    .Append("=")
                    .Append(p.GetValue(Target)?.ToString()??"null")
                    .Append("\n");
            return sb.ToString();
        }
    }
}
