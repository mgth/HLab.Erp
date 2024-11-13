using HLab.Base.ReactiveUI;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using NPoco;

namespace HLab.Erp.Base.Data;

public class Continent : Entity, IEntityWithExportId, IListableModel
{
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

    [Ignore]
    public string ExportId => Code;
    [Ignore]
    public string Caption => Name;
    [Ignore]
    public string IconPath => $"icon/continent/{Name}";
}
