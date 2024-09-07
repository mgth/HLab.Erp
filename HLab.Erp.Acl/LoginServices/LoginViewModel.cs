using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Erp.Acl.AuditTrails;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using ReactiveUI;

namespace HLab.Erp.Acl.LoginServices;

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

        foreach (var connection in dataService.Connections)
        {
            Databases.Add(connection);
            AllowDatabaseSelection = true;
        }

        Database = DataService.Source;

        NumPadCommand = ReactiveCommand.CreateFromTask<string>(
            NumPad, 
            this.WhenAnyValue(
                e => e.Username, 
                e => e.Password,
                (userName, password) => userName.Length > 0 && password.Length > 0
                ));

        LoginCommand = ReactiveCommand.CreateFromTask(
            LoginAsync, 
            this.WhenAnyValue(
                e => e.Username, 
                e => e.Password, 
                (userName, password) => userName.Length > 0 && password.Length > 0
                ));

        CancelCommand = ReactiveCommand.CreateFromTask(CancelAsync);

    }
    public string Title => "{Connection}";

    public ObservableCollection<string> Databases { get; } = new();

    public ILocalizationService LocalizationService { get; }
    public IIconService IconService { get; }
    public object MainIcon { get; }
    public IDataService DataService { get; }
    public IApplicationInfoService InfoService { get; }

    public string Database
    {
        get => _database;
        set
        {
            if (!SetAndRaise(ref _database, value)) return;

            DataService.Source = value;
            InfoService.DataSource = value;
        }
    }
    string _database;
        

    public bool AllowDatabaseSelection
    {
        get => _allowDatabaseSelection;
        set => SetAndRaise(ref _allowDatabaseSelection, value);
    }
    bool _allowDatabaseSelection;

    public string PinView
    {
        get => _pinView;
        set => SetAndRaise(ref _pinView, value);
    }
    string _pinView = "";

    string _pin = "";
    public ICommand NumPadCommand { get; }

    async Task NumPad(string value)
    {
                if (_pin.Length > 4) _pin = "";
                _pin += value;

                PinView = new string('.', _pin.Length);

                if (_pin.Length != 4) return;
                Message = await Acl.Login(new NetworkCredential(Credential.UserName, _pin), true);
                _pin = "";
                PinView = "";
    }


    public ICommand LoginCommand { get; }
    async Task LoginAsync()
    {
        Message = "";
        Message = await Task.Run(() => Acl.Login(Credential));
    }
        
        
    public ICommand CancelCommand { get; }
    public object AllowThemeSelection { get; }

    Task CancelAsync()
    {
        Acl.CancelLogin();
        return Task.CompletedTask;
    }
}