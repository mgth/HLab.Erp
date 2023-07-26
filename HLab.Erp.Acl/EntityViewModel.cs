using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Input;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Mvvm.Application;
using HLab.Mvvm.ReactiveUI;
using HLab.Notify.PropertyChanged;
using MySql.Data.MySqlClient;
using ReactiveUI;

namespace HLab.Erp.Acl;

public class ListableEntityViewModel<T> : EntityViewModel<T>
    where T : class, IEntity<int>, INotifyPropertyChanged, IListableModel
{
    protected ListableEntityViewModel():base(null){}

    public ListableEntityViewModel(Injector i) : base(i) 
    {
        _header = this.WhenAnyValue(
            e => e.EntityName, 
            e => e.Model.Caption, 
            selector: (n, c) => $"{n}\n{c}")
        .ToProperty(this, nameof(Header));

        _iconPath = this.WhenAnyValue(e => e.Model.IconPath)
            .ToProperty(this, nameof(IconPath));
    }

    public override object Header => _header.Value;
    readonly ObservableAsPropertyHelper<string> _header;

    public override string IconPath => _iconPath.Value;

    readonly ObservableAsPropertyHelper<string> _iconPath;

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

        _locker = this
            .WhenAnyValue(e => e.Model)
            .WhereNotNull()
            .Select(GetLocker)
        .ToProperty(this, nameof(Locker));

        _editAllowed = this
            .WhenAnyValue(e => e.Injected.Acl)
            .Select(acl => acl.IsGranted(EditRight))
            .ToProperty(this, nameof(EditAllowed));

       this.WhenAnyValue(e => e.Locker, e => e.EditAllowed) 
            .Subscribe(e => e.Item1.IsEnabled = e.Item2);

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

    ObservableAsPropertyHelper<IDataLocker<T>> _locker;
    private IDataLocker<T> GetLocker(T model)
    {
        var locker = Injected.GetLocker(model);
        locker.PropertyChanged += Locker_PropertyChanged;
        locker.BeforeSavingAction = BeforeSaving;
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

    public virtual AclRight EditRight => null;
    public virtual bool EditAllowed => _editAllowed.Value;

    readonly ObservableAsPropertyHelper<bool> _editAllowed;


    public virtual string EntityName => "{" + typeof(T).Name + "}";
    public virtual object Header => EntityName;

    public virtual string IconPath => _iconPath.Value;

    readonly ObservableAsPropertyHelper<string> _iconPath;
        
    public ICommand CloseCommand { get; }

    // TODO : move to locker ?
    // TODO : make it async ?
    private void Close()
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
