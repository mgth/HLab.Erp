using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using ReactiveUI;

namespace HLab.Erp.Acl.AuditTrails;

public class AuditTrailMotivationViewModel : AuthenticationViewModel, IAuditTrailProvider
{
    public IIconService IconService { get; }
    public ILocalizationService LocalizationService { get; }

    public string Title => EntityCaption;

    readonly IMvvmService _mvvm;
    readonly IDataTransaction _transaction;

    public AuditTrailMotivationViewModel(
        IAclService acl,
        IDataTransaction transaction, 
        IMvvmService mvvm, 
        IIconService icon, 
        ILocalizationService localization) : base(acl)
    {
        IconService = icon;
        LocalizationService = localization;
        _mvvm = mvvm;
        _transaction = transaction;
        User = Acl.Connection.User;

        _motivationNeeded = this.WhenAnyValue(e => e.MotivationMandatory, e => e.Motivation,
                selector: (motivationMandatory,motivation) 
                    => motivationMandatory && string.IsNullOrWhiteSpace(motivation))
            .ToProperty(this, e => e.MotivationNeeded);

        OkCommand = ReactiveCommand.CreateFromTask(Ok,this.WhenAnyValue(e => e.MotivationMandatory,e => e.Motivation, OkCanExecute));
        CancelCommand = ReactiveCommand.Create(Cancel,this.WhenAnyValue(e => e.Username, CancelCanExecute));
    }

    public bool MotivationMandatory
    {
        get => _motivationMandatory;
        set => SetAndRaise(ref _motivationMandatory,value);
    }
    bool _motivationMandatory ;

    public bool MotivationNeeded => _motivationNeeded.Value;
    readonly ObservableAsPropertyHelper<bool> _motivationNeeded;

    public bool Signing
    {
        get => _signing;
        set => SetAndRaise(ref _signing,value);
    }
    bool _signing = false;

    public string Motivation
    {
        get => _motivation;
        set => SetAndRaise(ref _motivation,value);
    }
    string _motivation = "";

    public string IconPath
    {
        get => _iconPath;
        set => SetAndRaise(ref _iconPath,value);
    }
    string _iconPath = "";

    public string EntityCaption
    {
        get => _entityCaption;
        set => SetAndRaise(ref _entityCaption,value);
    }
    string _entityCaption = "";

    public string Log
    {
        get => _log;
        set => SetAndRaise(ref _log,value);
    }
    string _log = "";

    public bool? Result
    {
        get => _result;
        set => SetAndRaise(ref _result,value);
    }
    bool? _result = false;

    public ICommand OkCommand { get; }
        
    // TODO : minimum length of motivation should be configurable
    static bool OkCanExecute(bool motivationMandatory, string motivation) => !motivationMandatory || (motivation?.Length ?? 0) >= 5;
    async Task Ok()
    {
        if(Signing)
        {
            var user = await Acl.Check(Credential);
            if(user==null)
            {
                Message = "Login ou mot de passe incorrect.";
                return;
            }
        }
        Message = "";
        Result = true;
    }

    void Cancel()
    {
        Message = "Action canceled";
        Result = false;
    }
    public ICommand CancelCommand { get; }

    static bool CancelCanExecute(string username) => (username?.Length??0) > 0;


    public bool Audit(string action, AclRight rightNeeded, string log, object entity, string caption, string iconPath, bool sign, bool motivate)
    {
        Log = log;
        MotivationMandatory = motivate;
        Signing = sign;

        EntityCaption = caption;
        IconPath = iconPath;
        SetPassword(new System.Security.SecureString());

        try
        {
            /* TODO
            var view = _mvvm.MainContext.GetView<DefaultViewMode>(this).AsDialog();
            if(view.ShowDialog()??false)
            {
                var audit = _transaction.Add<AuditTrail>(e =>
                {
                    e.Action = action;
                    e.Motivation = Motivation;
                    e.Log = log;
                    e.TimeStamp = DateTime.Now;
                    e.UserId = User.Id;
                    e.UserCaption = User.Caption;
                    e.EntityCaption = EntityCaption;
                    e.IconPath = IconPath;
                    e.EntityClass = entity.GetType().Name;

                    if(entity is IEntity<int> entityInt)
                        e.EntityId = entityInt.Id;

                    e.Failed = false;
                });
                if(audit!=null) return true;
                Message = "Failed to write audit entry";
            }
            */
            return false;

        }
        finally
        {
            SetPassword(new System.Security.SecureString());
        }
    }

}