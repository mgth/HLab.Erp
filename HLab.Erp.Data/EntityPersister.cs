using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Data
{
    public class EntityPersister : Persister
//    where T : class, INotifyPropertyChanged
    {
        [Import]
        private DataService _db;

        public EntityPersister(object target) : base(target)
        {
        }

//        protected new T Target => (T)base.Target;

        protected override Persistency CheckPersistency(PropertyInfo property)
        {
            if (!property.CanWrite) return Persistency.None;

            foreach (var attr in property.GetCustomAttributes().OfType<IgnoreAttribute>())
            {
                return Persistency.None;
            }

            return Persistency.OnSave;
        }
        public override void Save()
        {
            var columns = new List<string>();
            while (!Dirty.IsEmpty)
            {
                if (Dirty.TryTake(out var e))
                {
                    columns.Add(e.Name);
                }
            }

            using var db = _db.Get();
            db.Update(Target, columns);
        }
    }
}
