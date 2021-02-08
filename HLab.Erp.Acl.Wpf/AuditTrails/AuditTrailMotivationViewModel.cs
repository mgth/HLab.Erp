using System;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Views;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl.AuditTrails
{
    using H = H<AuditTrailMotivationViewModel>;

    [Export(typeof(IAuditTrailProvider))]
    public class AuditTrailMotivationViewModel : AuthenticationViewModel, IAuditTrailProvider
    {
        [Import]
        public IIconService IconService {get;}
        [Import]
        public ILocalizationService LocalizationService {get;}

        [Import]
        private IMvvmService _mvvm;

        private IDataTransaction _transaction;

        public AuditTrailMotivationViewModel(IDataTransaction transaction)
        {
            _transaction = transaction;
            H.Initialize(this);
            User = Acl.Connection.User;
        }

        public bool MotivationMandatory
        {
            get => _motivationMandatory.Get();
            set => _motivationMandatory.Set(value);
        }
        private readonly IProperty<bool> _motivationMandatory = H.Property<bool>();
        public bool Signing
        {
            get => _signing.Get();
            set => _signing.Set(value);
        }
        private readonly IProperty<bool> _signing = H.Property<bool>();

        public string Motivation
        {
            get => _motivation.Get();
            set => _motivation.Set(value);
        }
        private readonly IProperty<string> _motivation = H.Property<string>();
        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }
        private readonly IProperty<string> _iconPath = H.Property<string>();
        public string EntityCaption
        {
            get => _entityCaption.Get();
            set => _entityCaption.Set(value);
        }
        private readonly IProperty<string> _entityCaption = H.Property<string>();

        public string Log
        {
            get => _log.Get();
            set => _log.Set(value);
        }
        private readonly IProperty<string> _log = H.Property<string>();

        public bool? Result
        {
            get => _result.Get();
            set => _result.Set(value);
        }
        private readonly IProperty<bool?> _result = H.Property<bool?>();

        public ICommand OkCommand { get; } = H.Command( c => c
            .CanExecute(e => !e.MotivationMandatory || (e.Motivation?.Length??0)>=5)
            .Action(async e =>
                {
                    var user = await e.Acl.Check(e.Credential);
                    if(user==null)
                    {
                        e.Message = "Login ou mot de passe incorrect.";
                        return;
                    }
                    e.Message = "";
                    e.Result = true;
                })                      
            .On(e => e.Motivation).On(e => e.MotivationMandatory).CheckCanExecute()
        );
        public ICommand CancelCommand { get; } = H.Command( c => c
            .CanExecute(e => (e.Login?.Length??0)>0)
            .Action(async e =>
                {
                    e.Message = "Action canceled";
                    e.Result = false;
                })                      
            .On(e => e.Login).CheckCanExecute()
        );

        public bool Audit(string action, AclRight rightNeeded, string log, object entity, bool sign, bool motivate)
        {
            Log = log;
            MotivationMandatory = motivate;
            Signing = sign;

            if(entity is IListableModel lm)
            { 
                EntityCaption = lm.Caption; 
                IconPath = lm.IconPath;
            }

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

    }
}
