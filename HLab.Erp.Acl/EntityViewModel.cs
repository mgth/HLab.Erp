using System;
using System.ComponentModel;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl
{
    public class EntityViewModel<TClass, T> : ViewModel<TClass, T>
        where TClass : EntityViewModel<TClass, T>
        where T : class, IEntity<int>, INotifyPropertyChanged
    {
        
        [Import] private readonly Func<T, DataLocker<T>> _getLocker;
        public DataLocker<T> Locker => _locker.Get();

        private readonly IProperty<DataLocker<T>> _locker = H.Property<DataLocker<T>>(c => c
            .On(e => e.Model)
            .Set(e => e._getLocker(e.Model))
            .Set(e => e._getLocker(e.Model))
        );


    }
}
