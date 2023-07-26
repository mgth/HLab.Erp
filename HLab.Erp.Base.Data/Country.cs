using HLab.Erp.Data;
using HLab.Mvvm.Application;
using ReactiveUI;

namespace HLab.Erp.Base.Data;

public class Country : Entity,IListableModel
{
    public Country()
    {
        _continent = Foreign(this, e => e.ContinentId, e => e.Continent);

        _caption = this.WhenAnyValue(e => e.Name, selector: e => string.IsNullOrWhiteSpace(e)?"{New country}":e)
            .ToProperty(this, nameof(Caption), deferSubscription: true);
    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name,value);
    }

    string _name = "";
    public string IsoA2
    {
        get => _isoA2;
        set => this.RaiseAndSetIfChanged(ref _isoA2,value);
    }

    string _isoA2;
    public string IsoA3
    {
        get => _isoA3;
        set => this.RaiseAndSetIfChanged(ref _isoA3,value);
    }

    string _isoA3;

    public int Iso
   {
       get => _iso;
       set => this.RaiseAndSetIfChanged(ref _iso,value);
   }
    int _iso;

    public string IconPath
    {
        get => _iconPath;
        set => this.RaiseAndSetIfChanged(ref _iconPath,value);
    }
    string _iconPath;

    public int? ContinentId
    {
        get => _continentId;
        set => this.RaiseAndSetIfChanged(ref _continentId,value);
    }
    int? _continentId;

    public Continent Continent
    {
        get => _continent.Value;
        set => ContinentId = value?.Id;
    }
    readonly ObservableAsPropertyHelper<Continent> _continent;

    public string Caption => _caption.Value;
    readonly ObservableAsPropertyHelper<string> _caption;
}
