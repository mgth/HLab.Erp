using System;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Base.Data
{
    using H = HD<UnitClass>;
    public class UnitClass : Entity, IListableModel
    {
        public UnitClass() => H.Initialize(this);

        public string Name
        {
            get => _name.Get(); set => _name.Set(value);
        }

        private readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));

        public string Symbol
        {
            get => _symbol.Get(); set => _symbol.Set(value);
        }

        private readonly IProperty<string> _symbol = H.Property<string>(c => c.Default(""));

        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c.Default(""));


        public bool IsRatio
        {
            get => _isRatio.Get(); 
            set => _isRatio.Set(value);
        }
        private readonly IProperty<bool> _isRatio = H.Property<bool>(c => c.Default(false));

        [Ignore]
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .Set(e => string.IsNullOrWhiteSpace(e.Name) ? "{New Unit}" : $"{e.Name}")
        );


        public static UnitClass DesignModel => new(){
            Name = "Mass",
            IconPath ="Icons/Entities/Units/mass.svg"
            };


    }
}
