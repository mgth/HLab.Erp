using HLab.Erp.Data;
using HLab.Mvvm.Application;
using ReactiveUI;
using System.Reactive.Linq;
using HLab.Base.ReactiveUI;
using HLab.Erp.Data.foreigners;
using NPoco;

namespace HLab.Erp.Base.Data;

public class Country : Entity, IListableModel
{
    public Country() {
        _continent = this.Foreign( e => e.ContinentId, e => e.Continent);

        _caption = this.WhenAnyValue(e => e.Name)
            .Select(e => string.IsNullOrWhiteSpace(e) ? "{New country}" : e)
            .ToProperty(this, e => e.Caption);
    }

    public string Name
    {
        get => _name;
        set => this.SetAndRaise(ref _name,value);
    }
    string _name = "";

    public string IsoA2
    {
        get => _isoA2;
        set => this.SetAndRaise(ref _isoA2,value);
    }
    string _isoA2 = "";

    public string IsoA3
    {
        get => _isoA3;
        set => this.SetAndRaise(ref _isoA3,value);
    }
    string _isoA3 = "";

    public int Iso
   {
       get => _iso;
       set => this.SetAndRaise(ref _iso,value);
   }

    int _iso = 0;


    public string IconPath
    {
        get => _iconPath;
        set => this.SetAndRaise(ref _iconPath,value);
    }

    string _iconPath = "";

    public int? ContinentId
    {
        get => _continent.Id;
        set => _continent.SetId(value);
    }

    [Ignore]
    public Continent Continent
    {
        get => _continent.Value;
        set => ContinentId = value?.Id;
    }
    readonly ForeignPropertyHelper<Country,Continent> _continent;

    [Ignore]
    public string Caption => _caption.Value;

    readonly ObservableAsPropertyHelper<string> _caption;
}
