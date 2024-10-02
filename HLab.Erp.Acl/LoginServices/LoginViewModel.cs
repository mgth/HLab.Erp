using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Base.ReactiveUI;
using HLab.Erp.Acl.AuditTrails;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Options;
using HLab.UI;
using ReactiveUI;

namespace HLab.Erp.Acl.LoginServices;

public class LoginViewModel : AuthenticationViewModel, ILoginViewModel, IMainViewModel
{
    public LoginViewModel(
        IAclService acl,
        ILocalizationService localizationService,
        IIconService iconService,
        IDataService dataService,
        IApplicationInfoService infoService,
        IOptionsService options
        ) : base(acl)
    {
        UiPlatform.VerifyAccess();

        LocalizationService = localizationService;
        IconService = iconService;
        DataService = dataService;
        InfoService = infoService;

        foreach (var connection in dataService.Connections)
        {
            Databases.Add(connection);
            AllowDatabaseSelection = true;
        }

        this.WhenAnyValue(e => e.Database)
        .Select(database =>
        {
            DataService.Source = database;
            InfoService.DataSource = database;

#if DEBUG
            var path = (string.IsNullOrWhiteSpace(database)) ? "" : @$"Connections\{database}";
            Username = options.GetValue<string>(path, "DebugUsername", null, null, "registry");
            Password = options.GetValue<string>(path, "DebugPassword", null, null, "registry");
#endif

            return true;
        }).ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe();

        Database = DataService.Source;

        //NumPadCommand = ReactiveCommand.CreateFromTask<string>(
        //    NumPad,
        //    this.WhenAnyValue(
        //        e => e.Username,
        //        e => e.Password,
        //        (userName, password) => userName.Length > 0 && password.Length > 0
        //        ));

        LoginCommand = ReactiveCommand.CreateFromTask(
            LoginAsync
            , this.WhenAnyValue(
            e => e.Username,
            e => e.Password,
            (userName, password) => (userName??"").Length > 0 && (password??"").Length > 0
            )
        );

        CancelCommand = ReactiveCommand.CreateFromTask(CancelAsync);

    }
    public string Title => "{Connection}";

    public ObservableCollection<string> Databases { get; } = [];

    public ILocalizationService LocalizationService { get; }
    public IIconService IconService { get; }
    public object? MainIcon { get; } = null;
    public IDataService DataService { get; }
    public IApplicationInfoService InfoService { get; }

    public string Database
    {
        get => _database;
        set => this.SetAndRaise(ref _database, value);
    }
    string _database = "";


    public bool AllowDatabaseSelection
    {
        get => _allowDatabaseSelection;
        set => this.SetAndRaise(ref _allowDatabaseSelection, value);
    }
    bool _allowDatabaseSelection;

    public string PinView
    {
        get => _pinView;
        set => this.SetAndRaise(ref _pinView, value);
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
        Message = await Acl.LoginAsync(new NetworkCredential(Credential.UserName, _pin), true);
        _pin = "";
        PinView = "";
    }

    public ICommand? LoginCommand { get; private set;}
    async Task LoginAsync()
    {
        //await UiPlatform.InvokeOnUiThreadAsync(async () =>
        //    {
        //        Message = "";
        //        var credential = Credential;
        //        var message = await Acl.LoginAsync(credential);
        //        Message = message;
        //    }
        //);
        //        Message = await Task.Run(() => Acl.Login(Credential));
        // TODO : re
        //Message = await Acl.LoginAsync(Credential);
    }

    public ICommand CancelCommand { get; }
    public bool AllowThemeSelection { get; } = true;

    Task CancelAsync()
    {
        Acl.CancelLogin();
        return Task.CompletedTask;
    }
}