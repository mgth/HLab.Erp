using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HLab.Base;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Data
{
    public class EntityPersister<T> : Persister
    where T : class, IEntity
    {
        [Import]
        private IDataService _db;

        public EntityPersister(object target) : base(target,false)
        {
        }

        protected new T Target => (T)base.Target;

        protected override Persistency CheckPersistency(PropertyInfo property)
        {
            if (!property.CanWrite) return Persistency.None;

            return property.GetCustomAttributes().OfType<IgnoreAttribute>().Any() ? Persistency.None : Persistency.OnSave;
        }
        public override void Save()
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
                    var t = _db.Add<T>(e => Target.CopyPrimitivesTo(e));
                    ei.Id = (int)t.Id;
                    return;
                }
                _db.Update(Target, columns.Select(e => e.Name));
                IsDirty = false;
            }
            catch
            {
                foreach(var p in columns)
                    Dirty.Add(p);
                throw;
            }
        }
        public override async Task SaveAsync()
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
                    var t = await _db.AddAsync<T>(e => Target.CopyPrimitivesTo(e));
                    ei.Id = (int)t.Id;
                    return;
                }
                await _db.UpdateAsync(Target, columns.Select(e => e.Name));

                IsDirty = false;
            }
            catch
            {
                foreach (var p in columns)
                {
                    Dirty.Add(p);
                }
                throw;
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
