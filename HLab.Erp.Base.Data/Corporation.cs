using HLab.Erp.Data;

namespace HLab.Erp.Base.Data;

public abstract class Corporation : Entity, ICorporation 
{
    public Corporation() => _country = Foreign(this, e => e.CountryId, e => e.Country);

    public string Name
    {
        get => _name;
        set => SetAndRaise(ref _name,value);
    }
    string _name = "";

    public string Address
    {
        get => _address;
        set => SetAndRaise(ref _address,value);
    }
    string _address = "";

    public string Phone
    {
        get => _phone;
        set => SetAndRaise(ref _phone,value);
    }
    string _phone = "";

    public string Fax
    {
        get => _fax;
        set => SetAndRaise(ref _fax,value);
    }
    string _fax = "";

    public string Email
    {
        get => _email;
        set => SetAndRaise(ref _email,value);
    }
    string _email = "";

    public string Note
    {
        get => _note;
        set => SetAndRaise(ref _note,value);
    }
    string _note = "";

    public int? CountryId
    {
        get => _country.Id;
        set => _country.SetId(value);
    }
    public Country Country
    {
        get => _country.Value;
        set => CountryId = value.Id;
    }
    readonly ForeignPropertyHelper<Corporation,Country> _country;
}