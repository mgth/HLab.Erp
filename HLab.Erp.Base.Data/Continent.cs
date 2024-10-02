using HLab.Base.ReactiveUI;
using HLab.Erp.Data;
using HLab.Mvvm.Application;

namespace HLab.Erp.Base.Data;

public class Continent : Entity, IEntityWithExportId, IListableModel
{
    public Continent()
    {

    }

    public string Name
    {
        get => _name;
        set => this.SetAndRaise(ref _name,value);
    }

    string _name = "";
    public string Code
    {
        get => _code;
        set => this.SetAndRaise(ref _code,value);
    }

    string _code = "";
    public string ExportId => Code;
    public string Caption => Name;
    public string IconPath => $"icon/continent/{Name}";
}
