using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Input;
using HLab.Erp.Data;
using HLab.Mvvm.Application.Documents;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;

namespace HLab.Erp.Acl;

public class EntityViewModel<T> : ViewModel<T>
    where T : class, IEntity<int>, INotifyPropertyChanged
{

    public Injector Injected { get; }
    public class Injector(
        IDocumentService docs,
        Func<T, IDataLocker<T>> getLocker,
        IAclService acl,
        IDataService data)
    {
        public IDocumentService Docs => docs;
        public Func<T, IDataLocker<T>> GetLocker => getLocker;
        public IAclService Acl => acl;
        public IDataService Data => data;
    }

    public EntityViewModel(Injector injector)
    {
        Injected = injector;

        _locker = this
        .WhenAnyValue(e => e.Model, GetLocker)
        .WhereNotNull()
        .ToProperty(this, e => e.Locker, scheduler: RxApp.TaskpoolScheduler );

        _editAllowed = this
            .WhenAnyValue(e => e.Injected.Acl)
            .Select(acl => acl.IsGranted(EditRight))
            .ToProperty(this, e => e.EditAllowed);

       this.WhenAnyValue(e => e.Locker, e => e.EditAllowed) 
            .Do(args =>
            {
                var (locker, allowed) = args;
                if (locker == null) return;
                locker.IsEnabled = allowed;
            })
            .Subscribe();

    //    = H<EntityViewModel<T>>.Property<string>(c => c
    //.Set(e => "icons/entities/" + typeof(T).Name + (e.Locker.IsActive?"|Icons/Unlocked":""))
    //    .On(e => e.Locker.IsActive).Update()

        _iconPath = this.WhenAnyValue(e => e.Locker.IsActive)
            .Select(a => $"icons/entities/{typeof(T).Name}{(a ? "|Icons/Unlocked" : "")}")
            .ToProperty(this, nameof(IconPath));

    //     = H<EntityViewModel<T>>.Property<bool>(c => c
    //    .On(e => e.Locker.IsActive)
    //    .Set(e => !e.Locker.IsActive)
    //);

        _canClose = this.WhenAnyValue(e => e.Locker.IsActive).Select(a => !a).ToProperty(this, nameof(CanClose));

        CloseCommand = ReactiveCommand.Create(
            Close,
            this.WhenAnyValue(e => e.CanClose)
        );

    }

    public IDataLocker<T> Locker => _locker.Value;
    readonly ObservableAsPropertyHelper<IDataLocker<T>> _locker;

    public IDataLocker<T> LockerDone { get; set; }

    IDataLocker<T>? GetLocker(T? model)
    {
        if(model is null) return null;
        var locker = Injected.GetLocker(model);
        locker.PropertyChanged += Locker_PropertyChanged;
        locker.BeforeSavingAction = BeforeSaving;
        LockerDone = locker;
        return locker;
    }

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

    public virtual AclRight? EditRight => null;
    public virtual bool EditAllowed => _editAllowed.Value;

    readonly ObservableAsPropertyHelper<bool> _editAllowed;


    public virtual string EntityName => "{" + typeof(T).Name + "}";
    public virtual object Header => EntityName;

    public virtual string IconPath => _iconPath.Value;

    readonly ObservableAsPropertyHelper<string> _iconPath;
        
    public ICommand CloseCommand { get; }

    // TODO : move to locker ?
    // TODO : make it async ?
    void Close()
    {
            if (Locker.IsActive)
            {
                if (Locker.SaveCommand.CanExecute(null))
                {
                    return;
                    //e.Locker.SaveCommand.Execute(null);
                }
                else if (Locker.CancelCommand.CanExecute(null))
                    Locker.CancelCommand.Execute(null);
            }

            if (!Locker.IsActive)
            {
                Injected.Docs.CloseDocumentAsync(this);
            }
    }

    public bool CanClose => _canClose.Value;
    readonly ObservableAsPropertyHelper<bool> _canClose;
}
