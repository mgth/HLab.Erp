using System;
using System.ComponentModel;
using System.Windows.Input;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;


namespace HLab.Erp.Acl
{
    public class ListableEntityViewModel<T> : EntityViewModel<T>
        where T : class, IEntity<int>, INotifyPropertyChanged, IListableModel
    {
        public ListableEntityViewModel(Injector i) : base(i) 
            => H<ListableEntityViewModel<T>>.Initialize(this);

        public override object Header => _header.Get();

        readonly IProperty<string> _header = H<ListableEntityViewModel<T>>.Property<string>(c => c
            .Set(e => $"{e.EntityName}\n{e.Model.Caption}")
            .On(e => e.Model.Caption)
            .Update()
        );

        public override string IconPath => _iconPath.Get();

        readonly IProperty<string> _iconPath = H<ListableEntityViewModel<T>>.Property<string>(c => c
            .Set(e => e.Model.IconPath)
            .On(e => e.Model.IconPath)
            .Update()
        );

    }
    public class EntityViewModel<T> : ViewModel<T>
        where T : class, IEntity<int>, INotifyPropertyChanged
    {

        public Injector Injected { get; }
        public class Injector
        {
            public IDocumentService Docs { get; }
            public Func<T, IDataLocker<T>> GetLocker;
            public IAclService Acl { get; }
            public IDataService Data { get; }
            public Injector(IDocumentService docs, Func<T, IDataLocker<T>> getLocker, IAclService acl,
                IDataService data)
            {
                Docs = docs;
                GetLocker = getLocker;
                Acl = acl;
                Data = data;
            }
        }


        public EntityViewModel(Injector injector)
        {
            Injected = injector;
            
            H<EntityViewModel<T>>.Initialize(this);
        }

        public IDataLocker<T> Locker => _locker.Get();

        readonly IProperty<IDataLocker<T>> _locker = H<EntityViewModel<T>>.Property<IDataLocker<T>>(c => c
                .NotNull(e => e.Model)
                .Set(e =>
                {
                    var locker = e.Injected.GetLocker(e.Model);
                    locker.PropertyChanged += e.Locker_PropertyChanged;
                    locker.BeforeSavingAction = e.BeforeSaving;
                    return locker;
                })
                .On(e => e.Model)
                .NotNull(e => e.Model)
                .Update()
        );

        protected virtual void BeforeSaving(T entity)
        {

        }

        void Locker_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsActive") return;
            if (Locker.IsActive) return;
            if(Model.Id==-1)
            {
                CloseCommand.Execute(null);
            }
        }

        public virtual AclRight EditRight => null;
        public virtual bool EditAllowed => _editAllowed.Get();

        readonly IProperty<bool> _editAllowed = H<EntityViewModel<T>>.Property<bool>(c => c
            .Set(e => e.Injected.Acl.IsGranted(e.EditRight))
        );

        ITrigger _editAllowedTrigger = H<EntityViewModel<T>>.Trigger(c => c
            .On(e => e.EditAllowed)
            .On(e => e.Locker)
            .NotNull(e => e.Locker)
            .Do(e => e.Locker.IsEnabled = e.EditAllowed)
        );

        public virtual string EntityName => "{" + typeof(T).Name + "}";
        public virtual object Header => EntityName;

        public virtual string IconPath => _iconPath.Get();

        readonly IProperty<string> _iconPath = H<EntityViewModel<T>>.Property<string>(c => c
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
                    e.Injected.Docs.CloseDocumentAsync(e);
                }
            })
        );

        public bool CanClose => _canClose.Get();

        readonly IProperty<bool> _canClose = H<EntityViewModel<T>>.Property<bool>(c => c
            .On(e => e.Locker.IsActive)
            .Set(e => !e.Locker.IsActive)
        );
    }
}
