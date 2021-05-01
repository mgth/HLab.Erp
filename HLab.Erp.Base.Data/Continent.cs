using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using NPoco;

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

        private readonly IProperty<string> _name = HD<Continent>.Property<string>(c => c.Default(""));
        public string Code
        {
            get => _code.Get();
            set => _code.Set(value);
        }

        private readonly IProperty<string> _code = HD<Continent>.Property<string>(c => c.Default(""));
        [Ignore] public string ExportId => Code;
        [Ignore] public string Caption => Name;
        [Ignore] public string IconPath => $"icon/continent/{Name}";
    }
}
