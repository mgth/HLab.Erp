using System;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl
{
    public class EntityViewModel<TClass, T> : ViewModel<TClass, T>
        where TClass : EntityViewModel<TClass, T>
        where T : IEntity
    {
        
        [Import] private readonly Func<IEntity, DataLocker> _getLocker;

        public DataLocker Locker => _locker.Get();

        private readonly IProperty<DataLocker> _locker = H.Property<DataLocker>(c => c
            .On(e => e.Model)
            .Set(e => e._getLocker(e.Model))
            .Set(e => e._getLocker(e.Model))
        );
    }
}
