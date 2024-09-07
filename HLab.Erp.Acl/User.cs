using System;
using System.Reactive.Linq;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Acl;

public class User : Entity, IListableModel
{
    public User()
    {
        _caption = this
            .WhenAnyValue(e => e.Name)
            .Select(e => string.IsNullOrWhiteSpace(e)?"{New user}":e)
        .ToProperty(this,nameof(Caption));
    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    string _name = string.Empty;

    public string FirstName
    {
        get => _firstName;
        set => this.RaiseAndSetIfChanged(ref _firstName, value);
    }
    string _firstName = string.Empty;

    public string Initials
    {
        get => _initials;
        set => this.RaiseAndSetIfChanged(ref _initials, value);
    }
    string _initials = string.Empty;

    [Column("Login")]
    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }
    string _username = string.Empty; 

    public string Domain
    {
        get => _domain;
        set => this.RaiseAndSetIfChanged(ref _domain, value);
    }
    string _domain = "";

    public string HashedPassword
    {
        get => _hashedPassword;
        set => this.RaiseAndSetIfChanged(ref _hashedPassword, value);
    }
    string _hashedPassword = "";


    public string Function
    {
        get => _function;
        set => this.RaiseAndSetIfChanged(ref _function, value);
    }

    string _function = string.Empty;

    public string Phone
    {
        get => _phone;
        set => this.RaiseAndSetIfChanged(ref _phone, value);
    }

    string _phone = string.Empty;

    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    string _email = string.Empty;

    public string Note
    {
        get => _note;
        set => this.RaiseAndSetIfChanged(ref _note, value);
    }

    string _note = string.Empty;

    public string Pin
    {
        get => _pin;
        set => this.RaiseAndSetIfChanged(ref _pin, value);
    }

    string _pin = string.Empty;

    public DateTime? Expiry
    {
        get => _expiry;
        set => this.RaiseAndSetIfChanged(ref _expiry, value);
    }

    DateTime? _expiry;

    [Ignore]
    public string Caption => _caption.Value;
    ObservableAsPropertyHelper<string> _caption;

    [Ignore]
    public string IconPath => "Icon/User";

    public static User DesignModel => new User()
    {
        Name = "Ouedraogo",
        FirstName = "Michel",
        Initials = "MO",
        Username="o.ouedraogo",
        Domain="hlab.org",
        Function = "Technician",
        Phone = "+200 547 684",
        Email = "o.ouedraogo@hlab.org",

    };
}
