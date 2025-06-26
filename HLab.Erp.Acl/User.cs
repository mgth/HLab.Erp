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

    public string Name { get; set => this.RaiseAndSetIfChanged(ref field, value); } = string.Empty;

    public string FirstName { get; set => this.RaiseAndSetIfChanged(ref field, value); } = string.Empty;

    public string Initials { get; set => this.RaiseAndSetIfChanged(ref field, value); } = string.Empty;

    [Column("Login")]
    public string Username { get; set => this.RaiseAndSetIfChanged(ref field, value); } = string.Empty;

    public string Domain { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";

    public string HashedPassword { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";


    public string Function { get; set => this.RaiseAndSetIfChanged(ref field, value); } = string.Empty;

    public string Phone { get; set => this.RaiseAndSetIfChanged(ref field, value); } = string.Empty;

    public string Email { get; set => this.RaiseAndSetIfChanged(ref field, value); } = string.Empty;

    public string Note { get; set => this.RaiseAndSetIfChanged(ref field, value); } = string.Empty;

    public string Pin { get; set => this.RaiseAndSetIfChanged(ref field, value); } = string.Empty;

    public DateTime? Expiry { get; set => this.RaiseAndSetIfChanged(ref field, value); }

    [Ignore]
    public string Caption => _caption.Value;

    readonly ObservableAsPropertyHelper<string> _caption;

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
