using System;
using System.ComponentModel;
using System.Windows.Input;
using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl
{
    public class EntityViewModel<T> : ViewModel<T>
        where T : class, IEntity<int>, INotifyPropertyChanged
    {
        private IDocumentService _docs;
        
        private Func<T, DataLocker<T>> _getLocker;
        protected IAclService Acl;

        [Import] public void Inject(IDocumentService docs, Func<T, DataLocker<T>> getLocker, IAclService acl)
        {
            _docs = docs;
            _getLocker = getLocker;
            Acl = acl;
        }

        public EntityViewModel()
        {
            H<EntityViewModel<T>>.Initialize(this);
        }

        public DataLocker<T> Locker => _locker.Get();

        private readonly IProperty<DataLocker<T>> _locker = H<EntityViewModel<T>>.Property<DataLocker<T>>(c => c
            .Set(e => e.GetLocker())
            .On(e => e.Model)
            .NotNull(e => e.Model)
            .Update()
        );

        private DataLocker<T> GetLocker()
        {
            if (Model == null) return null;
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
        private readonly IProperty<bool> _editAllowed = H<EntityViewModel<T>>.Property<bool>(c => c
            .Set(e => e.Acl.IsGranted(e.EditRight))
        );

        private IProperty<bool> _onEditAllowed = H<EntityViewModel<T>>.Property<bool>(c => c
            .On(e => e.EditAllowed)
            .On(e => e.Locker)
            .NotNull(e => e.Locker)
            .Do(e => e.Locker.IsEnabled = e.EditAllowed)
        );

        public virtual string EntityName => "{" + typeof(T).Name + "}";
        public virtual object Header => EntityName;

        public virtual string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H<EntityViewModel<T>>.Property<string>(c => c
        .Set(e => "icons/entities/" + typeof(T).Name + (e.Locker.IsActive?"|Icons/Unlocked":""))
            .On(e => e.Locker.IsActive).Update()

        );
            
        public ICommand CloseCommand { get; } = H<EntityViewModel<T>>.Command(c => c
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
        private readonly IProperty<bool> _canClose = H<EntityViewModel<T>>.Property<bool>(c => c
            .On(e => e.Locker.IsActive)
            .Set(e => !e.Locker.IsActive)
        );
    }
}
