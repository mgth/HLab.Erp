using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Acl;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Base.Data
{

    public class Country : Entity<Country>
    {
        [System.ComponentModel.DataAnnotations.Schema.Column]
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));
        [Ignore] 
        public string IsoA2
        {
            get => _isoA2.Get();
            set => _isoA2.Set(value);
        }
        private readonly IProperty<string> _isoA2 = H.Property<string>();
        public string IsoA3
        {
            get => _isoA3.Get();
            set => _isoA3.Set(value);
        }
        private readonly IProperty<string> _isoA3 = H.Property<string>();

        public int Iso
       {
           get => _iso.Get();
           set => _iso.Set(value);
       }
       private readonly IProperty<int> _iso = H.Property<int>();


        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }
        private readonly IProperty<string> _iconPath = H.Property<string>();


        public int? ContinentId
        {
            get => _continentId.Get();
            set => _continentId.Set(value);
        }
        private readonly IProperty<int?> _continentId = H.Property<int?>();

        [Ignore]
        public Continent Continent
        {
            //get => E.GetForeign<Continent>(() => ContinentId);
            set => ContinentId = value.Id;
            get => _continent.Get();
        }
        readonly IProperty<Continent> _continent = H.Property<Continent>(c => c
            .Foreign(e => e.ContinentId));
    }
}
