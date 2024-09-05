using HLab.Erp.Data;
using HLab.Mvvm.Application;
using ReactiveUI;

namespace HLab.Erp.Base.Data;

public class Customer : Corporation, ILocalCache, IListableModel
{
    public Customer()
    {
        _caption = this
            .WhenAnyValue(e => e.Name,e => e.Id, selector: (name,id) => (id < 0 && string.IsNullOrEmpty(name)) ? "{New customer}" : name)
            .ToProperty(this, nameof(Caption));

        _iconPath = this
            .WhenAnyValue(e => e.Country.IconPath)
            .ToProperty(this, nameof(IconPath));
    }

    public string Caption => _caption.Value;
    readonly ObservableAsPropertyHelper<string> _caption;

    public string IconPath => _iconPath.Value;
    readonly ObservableAsPropertyHelper<string> _iconPath;

    public static Customer GetDesignModel()
    {
        return new Customer
        {
            Name = "Dummy Customer",
            Address = "Somewhere in the world\n10000 NOWHERE",
            Phone = "+33 6 123 123"
        };
    }
}
