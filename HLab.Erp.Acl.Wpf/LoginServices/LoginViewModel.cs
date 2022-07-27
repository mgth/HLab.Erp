using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Erp.Acl.AuditTrails;
using HLab.Erp.Data;
using HLab.Icons.Annotations.Icons;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl.LoginServices
{
    using H = H<LoginViewModel>;

    public class LoginViewModel : AuthenticationViewModel, ILoginViewModel, IMainViewModel
    {
        public LoginViewModel(
            IAclService acl,
            ILocalizationService localizationService, 
            IIconService iconService, 
            IDataService dataService, 
            IApplicationInfoService infoService) : base(acl)
        {
            LocalizationService = localizationService;
            IconService = iconService;
            DataService = dataService;
            InfoService = infoService;

            H.Initialize(this);

            foreach (var connection in dataService.Connections)
            {
                Databases.Add(connection);
                AllowDatabaseSelection = true;
            }

            Database = DataService.Source;
        }

        public string Title => "{Connection}";

        public ObservableCollection<string> Databases { get; } = new();
        
        public ILocalizationService LocalizationService { get; private set; }
        public IIconService IconService {get; private set; }
        public IDataService DataService {get; private set; }
        public IApplicationInfoService InfoService {get; private set; }

        public string Database
        {
            get => _database.Get();
            set
            {
                if (_database.Set(value))
                {
                    DataService.Source = value;
                    InfoService.DataSource = value;
                }
            }
        }

        readonly IProperty<string> _database = H.Property<string>();

        public bool AllowDatabaseSelection
        {
            get => _allowDatabaseSelection.Get();
            set => _allowDatabaseSelection.Set(value);
        }

        readonly IProperty<bool> _allowDatabaseSelection = H.Property<bool>();

        public string PinView
        {
            get => _pinView.Get();
            set => _pinView.Set(value);
        }

        readonly IProperty<string> _pinView = H.Property<string>();

        string _pin = "";
        public ICommand NumPadCommand { get; } = H.Command(c => c
            
            .CanExecute((e) => (e.Login?.Length ?? 0) > 0)
            .Action(
            async (e,n) =>
            {
                if (e._pin.Length > 4) e._pin = "";
                e._pin += (string)n;

                e.PinView = new String('.', e._pin.Length);

                if (e._pin.Length == 4)
                {
                    e.Message = await e.Acl.Login(new NetworkCredential(e.Credential.UserName, e._pin), true);
                    e._pin = "";
                    e.PinView = "";
                }
            }
            )
            .On(e => e.Login)
            .On(e => e.Password)
            .CheckCanExecute()
        );

        public ICommand LoginCommand { get; } = H.Command( c => c
            .CanExecute(e => (e.Login?.Length??0)>0)
            .Action(async e =>
                {
                    e.Message = await Task.Run(()=>e.Acl.Login(e.Credential));
                })                      
            .On(e => e.Login).CheckCanExecute()
        );
        public ICommand CancelCommand { get; } = H.Command( c => c
            .CanExecute(e => true)
            .Action( e =>
            {
                e.Acl.CancelLogin();
            })                      
//            .On(e => e.Login).CheckCanExecute()
        );

    }
}
