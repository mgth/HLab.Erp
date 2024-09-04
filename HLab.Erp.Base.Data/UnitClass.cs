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
        public UnitClass() { }

        public string Name
        {
            get => _name; set => this.RaiseAndSetIfChanged(ref _name,value);
        }

        string _name = "";

        public string Symbol
        {
            get => _symbol; set => this.RaiseAndSetIfChanged(ref _symbol,value);
        }

        string _symbol = "";

        public string IconPath
        {
            get => _iconPath;
            set => this.RaiseAndSetIfChanged(ref _iconPath,value);
        }

        string _iconPath = "";


        public bool IsRatio
        {
            get => _isRatio; 
            set => this.RaiseAndSetIfChanged(ref _isRatio,value);
        }

        bool _isRatio = false;

        [Ignore]
        public string Caption => _caption.Get();

        string _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .Set(e => string.IsNullOrWhiteSpace(e.Name) ? "{New Unit}" : $"{e.Name}")
        );


        public static UnitClass DesignModel => new(){
            Name = "Mass",
            IconPath ="Icons/Entities/Units/mass.svg"
            };


    }
}
