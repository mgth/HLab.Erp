using HLab.Erp.Data;

using System;

namespace HLab.Erp.Acl
{
    public interface IDataLocker<T> : IDataLocker
    where T : class,IEntity<int>
    {
            Action<T> BeforeSavingAction { get; set; }
            EntityPersister<T> Persister { get; }
    }
}