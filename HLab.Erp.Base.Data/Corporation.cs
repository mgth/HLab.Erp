using HLab.Base.ReactiveUI;
using HLab.Erp.Data;
using HLab.Erp.Data.foreigners;
using NPoco;

namespace HLab.Erp.Base.Data;

public abstract class Corporation : Entity, ICorporation 
{
    protected Corporation() => _country = this.Foreign( e => e.CountryId, e => e.Country);

    public string Name
    {
        get => _name;
        set => this.SetAndRaise(ref _name,value);
    }
    string _name = "";

    public string Address
    {
        get => _address;
        set => this.SetAndRaise(ref _address,value);
    }
    string _address = "";

    public string Phone
    {
        get => _phone;
        set => this.SetAndRaise(ref _phone,value);
    }
    string _phone = "";

    public string Fax
    {
        get => _fax;
        set => this.SetAndRaise(ref _fax,value);
    }
    string _fax = "";

    public string Email
    {
        get => _email;
        set => this.SetAndRaise(ref _email,value);
    }
    string _email = "";

    public string Note
    {
        get => _note;
        set => this.SetAndRaise(ref _note,value);
    }
    string _note = "";

    public int? CountryId
    {
        get => _country.Id;
        set => _country.SetId(value);
    }

    [Ignore]
    public Country Country
    {
        get => _country.Value;
        set => CountryId = value.Id;
    }
    readonly ForeignPropertyHelper<Corporation,Country> _country;
}