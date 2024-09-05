using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using ReactiveUI;

namespace HLab.Erp.Base.Data;

public class Continent : Entity, IEntityWithExportId, IListableModel
{
    public Continent()
    {

    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    string _name = "";

    public string Code
    {
        get => _code;
        set => this.RaiseAndSetIfChanged(ref _code, value);
    }
    string _code = "";

    // Todo make it reactive
    public string ExportId => Code;
    public string Caption => Name;
    public string IconPath => $"icon/continent/{Name}";
}
