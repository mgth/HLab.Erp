using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Data
{
    public class Continent : Entity, IEntityWithExportId, IListableModel
    {
        public Continent() => HD<Continent>.Initialize(this);

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = HD<Continent>.Property<string>(c => c.Default(""));
        public string Code
        {
            get => _code.Get();
            set => _code.Set(value);
        }

        readonly IProperty<string> _code = HD<Continent>.Property<string>(c => c.Default(""));
        public string ExportId => Code;
        public string Caption => Name;
        public string IconPath => $"icon/continent/{Name}";
    }
}
