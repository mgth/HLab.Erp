using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Input;
using HLab.Mvvm.Views;

namespace HLab.Erp.Acl
{
    [Export(typeof(IAuditTrailProvider))]
    public class AuditTrailMotivationViewModel : AuthenticationViewModel<AuditTrailMotivationViewModel>, IAuditTrailProvider
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
            User = Acl.Connection.User;
        }

        public bool MotivationMandatory
        {
            get => _motivationMandatory.Get();
            set => _motivationMandatory.Set(value);
        }
        private IProperty<bool> _motivationMandatory = H.Property<bool>();

        public string Motivation
        {
            get => _motivation.Get();
            set => _motivation.Set(value);
        }
        private IProperty<string> _motivation = H.Property<string>();
        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }
        private IProperty<string> _iconPath = H.Property<string>();
        public string EntityCaption
        {
            get => _entityCaption.Get();
            set => _entityCaption.Set(value);
        }
        private IProperty<string> _entityCaption = H.Property<string>();

        public string Log
        {
            get => _log.Get();
            set => _log.Set(value);
        }
        private IProperty<string> _log = H.Property<string>();

        public bool? Result
        {
            get => _result.Get();
            set => _result.Set(value);
        }
        private IProperty<bool?> _result = H.Property<bool?>();

        public ICommand OkCommand { get; } = H.Command( c => c
            .CanExecute(e => !e.MotivationMandatory || (e.Motivation?.Length??0)>0)
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

        public bool Audit(string action, AclRight rightNeeded, string log, object entity)
        {
            Log = log;

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

    public class AuthenticationViewModel<T> : ViewModel<T>
        where T : AuthenticationViewModel<T>
    {
        [Import] protected readonly IAclService Acl;


        public string Message
        {
            get => _message.Get();
            set => _message.Set(value);
        }
        private readonly IProperty<string> _message = H.Property<string>(nameof(Message));


        public void SetPassword(SecureString password)
        {
            Credential.SecurePassword = password;
        }

        public string Login
        {
            get => _login.Get();
            set
            {
                if(_login.Set(value))
                {
                    Credential.UserName = value;
                }
            }
        }
        private IProperty<string> _login = H.Property<string>(c => c
#if DEBUG
                    .Default("admin")
#else
                    .Default("")
#endif
        );

        public User User
        {
            get => _user.Get();
            set
            {
                if (_user.Set(value))
                {
                    Login = value?.Login;
                }
            }
        }
        private IProperty<User> _user = H.Property<User>();


        public string Password
        {
            get => _password.Get();
            set => _password.Set(value);
        }
        private IProperty<string> _password = H.Property<string>(c => c
#if DEBUG
        .Default("VEqwosdLL6ZPetwK5aFlIg")
#else
        .Default("")

#endif
        );

        public NetworkCredential Credential
        {
            get => _credential.Get();
            set
            {
                if(_credential.Set(value))
                {
                    Login = value?.UserName;
                    Password = Acl.Crypt(value.SecurePassword);
                }
            }
        }
        private readonly IProperty<NetworkCredential> _credential = H.Property<NetworkCredential>(c => c
#if DEBUG
        
        .Default(new NetworkCredential("admin", "blagueur"))
#else
        .Default(new NetworkCredential("", ""))
#endif
        );


#if DEBUG
        public Visibility DebugVisibility => Visibility.Visible;
#else
        public Visibility DebugVisibility => Visibility.Collapsed;
#endif
        public ObservableQuery<User> UserList => _userList.Get();
        private readonly IProperty<ObservableQuery<User>> _userList = H.Property<ObservableQuery<User>>(nameof(UserList));

        //TODO : respect template
        [Import(InjectLocation.AfterConstructor)]
        private void _setUserList(ObservableQuery<User> userlist)
        {
            _userList.Set(userlist
                .AddFilter(()=>u => u.Note.Contains("<balances>"))
                .FluentUpdate()            
            );
        }
    }
}
