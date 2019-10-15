using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Data
{
    public class Continent : Entity<Continent>
    {
        [Column("Nom")]
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        IProperty<string> _name = H.Property<string>(c => c.Default(""));
    }
}
