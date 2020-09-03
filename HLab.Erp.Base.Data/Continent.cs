using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Data
{
    public class Continent : Entity
    {
        public Continent() => HD<Continent>.Initialize(this);

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        private readonly IProperty<string> _name = HD<Continent>.Property<string>(c => c.Default(""));
    }
}
