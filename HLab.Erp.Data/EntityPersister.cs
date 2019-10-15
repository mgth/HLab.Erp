using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Data
{
    public class EntityPersister<T> : Persister
    where T : class, INotifyPropertyChanged
    {
        [Import]
        private DataService _db;

        public EntityPersister(T target) : base(target)
        {
        }

        protected new T Target => (T)base.Target;

        public override void Save()
        {
            using (var db = _db.Get())
            {
                //var entry = db.Attach(Target);
                //while (!Dirty.IsEmpty)
                //{
                //    if (Dirty.TryTake(out var e))
                //    {
                //        entry.Property(e.Name).IsModified = true;
                //    }
                //}

                db.Save(Target);
                //db.SaveChanges();
            }
            base.Save();
        }
    }
}
