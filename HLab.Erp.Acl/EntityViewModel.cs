using System;
using System.ComponentModel;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using Renci.SshNet;

namespace HLab.Erp.Acl
{
    public class EntityViewModel<TClass, T> : ViewModel<TClass, T>
        where TClass : EntityViewModel<TClass, T>
        where T : class, IEntity<int>, INotifyPropertyChanged
    {
        [Import] private readonly IDocumentService _docs;
        
        [Import] private readonly Func<T, DataLocker<T>> _getLocker;
        public DataLocker<T> Locker => _locker.Get();

        private readonly IProperty<DataLocker<T>> _locker = H.Property<DataLocker<T>>(c => c
            .On(e => e.Model)
            .NotNull(e => e.Model)
            .Set(e => e._getLocker(e.Model))
            //.Set(e => e._getLocker(e.Model))
        );

        public virtual string EntityName => "{" + typeof(T).Name + "}";

        public virtual string EntityIconPath => "icons/entities/" + typeof(T).Name;

        public ICommand CloseCommand { get; } = H.Command(c => c
            .Action(e =>
            {
                if (e.Locker.IsActive)
                {
                    if (e.Locker.SaveCommand.CanExecute(null))
                    {
                        return;
                        //e.Locker.SaveCommand.Execute(null);
                    }
                    else if (e.Locker.CancelCommand.CanExecute(null))
                        e.Locker.CancelCommand.Execute(null);
                }

                if (!e.Locker.IsActive)
                {
                    e._docs.CloseDocumentAsync(e);
                }
            })
        );

        public bool CanClose => _canClose.Get();
        private readonly IProperty<bool> _canClose = H.Property<bool>(c => c
            .On(e => e.Locker.IsActive)
            .Set(e => !e.Locker.IsActive)
        );
    }
}
