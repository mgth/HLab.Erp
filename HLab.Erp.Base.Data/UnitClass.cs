using HLab.Erp.Data;
using HLab.Mvvm.Application;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Base.Data;

public class UnitClass : Entity, IListableModel
{
    public UnitClass() 
    { 
        _caption = this
            .WhenAnyValue(e => e.Name, e => string.IsNullOrWhiteSpace(e)?"{New Unit}":e)
            .ToProperty(this, nameof(Caption));    
    }

    public string Name
    {
        get => _name; 
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    string _name = "";

    public string Symbol
    {
        get => _symbol; 
        set => this.RaiseAndSetIfChanged(ref _symbol, value);
    }

    string _symbol = string.Empty;

    public string IconPath
    {
        get => _iconPath;
        set => this.RaiseAndSetIfChanged(ref _iconPath, value);
    }

    string _iconPath = string.Empty;


    public bool IsRatio
    {
        get => _isRatio; 
        set => this.RaiseAndSetIfChanged(ref _isRatio, value);
    }
    bool _isRatio = false;

    [Ignore]
    public string Caption => _caption.Value;
    readonly ObservableAsPropertyHelper<string>_caption;

    public static UnitClass DesignModel => new(){
        Name = "Mass",
        IconPath ="Icons/Entities/Units/mass.svg"
        };


}
