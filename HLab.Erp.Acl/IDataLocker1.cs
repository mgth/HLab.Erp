using HLab.Erp.Data;

using System;
using System.ComponentModel;

namespace HLab.Erp.Acl
{
    public interface IDataLocker<T> : IDataLocker
    where T : class,IEntity<int>
    {
        event PropertyChangedEventHandler PropertyChanged;
        Action<T> BeforeSavingAction { get; set; }
        EntityPersister<T> Persister { get; }

    }
}