namespace HLab.Erp.Base.Data;

public interface ICorporation
{
    string Name { get; set; }
    string Address { get; set; }
    string Phone { get; set; }
    string Fax { get; set; }
    string Email { get; set; }
    string Note { get; set; }

    int? CountryId { get; set; }
    Country Country { get; set; }
}