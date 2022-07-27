using System;
using System.Windows.Input;
using HLab.Erp.Data;
using HLab.Icons.Annotations.Icons;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Views;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl.AuditTrails
{
    using H = H<AuditTrailMotivationViewModel>;

    public class AuditTrailMotivationViewModel : AuthenticationViewModel, IAuditTrailProvider, IMainViewModel
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
            H.Initialize(this);
            User = Acl.Connection.User;
        }

        public bool MotivationMandatory
        {
            get => _motivationMandatory.Get();
            set => _motivationMandatory.Set(value);
        }

        readonly IProperty<bool> _motivationMandatory = H.Property<bool>();
        public bool MotivationNeeded => _motivationNeeded.Get();

        readonly IProperty<bool> _motivationNeeded = H.Property<bool>(c => c
            .Set(e => e.MotivationMandatory && string.IsNullOrWhiteSpace(e.Motivation))
            .On(e => e.MotivationMandatory)
            .On(e => e.Motivation)
            .Update()
        );
        public bool Signing
        {
            get => _signing.Get();
            set => _signing.Set(value);
        }

        readonly IProperty<bool> _signing = H.Property<bool>();

        public string Motivation
        {
            get => _motivation.Get();
            set => _motivation.Set(value);
        }

        readonly IProperty<string> _motivation = H.Property<string>();
        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }

        readonly IProperty<string> _iconPath = H.Property<string>();
        public string EntityCaption
        {
            get => _entityCaption.Get();
            set => _entityCaption.Set(value);
        }

        readonly IProperty<string> _entityCaption = H.Property<string>();

        public string Log
        {
            get => _log.Get();
            set => _log.Set(value);
        }

        readonly IProperty<string> _log = H.Property<string>();

        public bool? Result
        {
            get => _result.Get();
            set => _result.Set(value);
        }

        readonly IProperty<bool?> _result = H.Property<bool?>();

        public ICommand OkCommand { get; } = H.Command( c => c
            .CanExecute(e => !e.MotivationMandatory || (e.Motivation?.Length??0)>=5)
            .Action(async e =>
                {
                    if(e.Signing)
                    {
                        var user = await e.Acl.Check(e.Credential);
                        if(user==null)
                        {
                            e.Message = "Login ou mot de passe incorrect.";
                            return;
                        }
                    }
                    e.Message = "";
                    e.Result = true;
                })                      
            .On(e => e.Motivation).On(e => e.MotivationMandatory).CheckCanExecute()
        );
        public ICommand CancelCommand { get; } = H.Command( c => c
            .CanExecute(e => (e.Login?.Length??0)>0)
            .Action(e =>
                {
                    e.Message = "Action canceled";
                    e.Result = false;
                })                      
            .On(e => e.Login).CheckCanExecute()
        );

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
                var view = _mvvm.MainContext.GetView<ViewModeDefault>(this).AsDialog();
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
                return false;
            }
            finally
            {
                SetPassword(new System.Security.SecureString());
            }
        }

    }
}
