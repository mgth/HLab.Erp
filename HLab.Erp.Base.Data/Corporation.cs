using HLab.Erp.Data;
using ReactiveUI;

namespace HLab.Erp.Base.Data;

public abstract class Corporation : Entity, ICorporation 
{
    protected Corporation() 
    {
        Foreign(this, e => e.CountryId, e => e.Country);
    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name,value);
    }

    string _name = "";
    public string Address
    {
        get => _address;
        set => this.RaiseAndSetIfChanged(ref _address,value);
    }

    string _address = "";
    public string Phone
    {
        get => _phone;
        set => this.RaiseAndSetIfChanged(ref _phone,value);
    }

    string _phone = "";
    public string Fax
    {
        get => _fax;
        set => this.RaiseAndSetIfChanged(ref _fax,value);
    }

    string _fax = "";
    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email,value);
    }

    string _email = "";
    public string Note
    {
        get => _note;
        set => this.RaiseAndSetIfChanged(ref _note,value);
    }

    string _note = "";

    public int? CountryId
    {
        get => _countryId;
        set => this.RaiseAndSetIfChanged(ref _countryId,value);
    }
    int? _countryId;

    public Country Country
    {
        get => _country.Value;
        set => CountryId = value.Id;
    }

    readonly ObservableAsPropertyHelper<Country>_country;
}