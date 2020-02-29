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
        [Import] protected IAclService Acl;


        public DataLocker<T> Locker => _locker.Get();

        private readonly IProperty<DataLocker<T>> _locker = H.Property<DataLocker<T>>(c => c
            .On(e => e.Model)
            .NotNull(e => e.Model)
            .Set(e => e.GetLocker())
            //.Set(e => e._getLocker(e.Model))
        );

        private DataLocker<T> GetLocker()
        {
            var locker = _getLocker(Model);
            locker.PropertyChanged += Locker_PropertyChanged;
            return locker;
        }

        private void Locker_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName=="IsActive")
            {
                if (Locker.IsActive==false)
                {
                    if(Model.Id==-1)
                    {
                        CloseCommand.Execute(null);
                    }
                }
            }
        }

        public virtual AclRight EditRight => null;

        public virtual bool EditAllowed => _editAllowed.Get();
        private IProperty<bool> _editAllowed = H.Property<bool>(c => c
            .Set(e => e.Acl.IsGranted(e.EditRight))
        );

        private IProperty<bool> _onEditAllowed = H.Property<bool>(c => c
            .On(e => e.EditAllowed)
            .On(e => e.Locker)
            .NotNull(e => e.Locker)
            .Do(e => e.Locker.IsEnabled = e.EditAllowed)
        );

        public virtual string EntityName => "{" + typeof(T).Name + "}";
        public virtual string Title => EntityName;

        public virtual string IconPath => "icons/entities/" + typeof(T).Name;

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
