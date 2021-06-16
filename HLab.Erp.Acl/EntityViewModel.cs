using System;
using System.ComponentModel;
using System.Windows.Input;
using HLab.Core.Annotations;
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
        public IAclService Acl { get; private set; }
        public IDataService Data { get; private set; }

        public void Inject(IDocumentService docs, Func<T, DataLocker<T>> getLocker, IAclService acl, IDataService data)
        {
            _docs = docs;
            _getLocker = getLocker;
            Acl = acl;
            Data = data;
        }

        public EntityViewModel() => H<EntityViewModel<T>>.Initialize(this);

        public DataLocker<T> Locker => _locker.Get();

        private readonly IProperty<DataLocker<T>> _locker = H<EntityViewModel<T>>.Property<DataLocker<T>>(c => c
                .NotNull(e => e.Model)
                .Set(e =>
                {
                    var locker = e._getLocker(e.Model);
                    locker.PropertyChanged += e.Locker_PropertyChanged;
                    return locker;
                })
                .On(e => e.Model)
                .NotNull(e => e.Model)
                .Update()
        );

        //public bool IsActive => _isActive.Get();
        //private readonly IProperty<bool> _isActive = H<EntityViewModel<T>>.Property<bool>(c => c
        //    .Set(e => e.Locker?.IsActive??false)
        //    .On(e => e.Locker.IsActive)
        //    .Update()
        //);


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
