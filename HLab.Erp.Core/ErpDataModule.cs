using System;
using System.Windows.Input;
using HLab.Base.Extensions;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;


namespace HLab.Erp.Core
{

    public abstract class NestedBootloader : NotifierBase, IBootloader
    {
        public IErpServices Erp { get; set; }

        protected NestedBootloader()
        {
            GetEntityName();
            H<NestedBootloader>.Initialize(this);
        }
        public virtual string Caption => Name.FromCamelCase();
        public virtual string Name => _parentType.Name.BeforeSuffix(_suffix);
        public virtual string Header => $"{{{Caption}}}";
        public virtual string IconPath => $"Icons/Entities/{_entityName}";
        public virtual string MenuPath => "data";
        public virtual bool Allowed => true;


        public ICommand OpenCommand { get; } = H<NestedBootloader>.Command(c => c
            .Action(e => e.Erp.Docs.OpenDocumentAsync(e.GetType().DeclaringType))
            .CanExecute(e => true)
        );

        string _suffix = "";
        string _entityName = "";

        Type _parentType;

        void GetEntityName()
        {
            _parentType = GetType().DeclaringType;

            _ = _parentType ?? throw new ArgumentException($"class {GetType().Name} must be used nested");

            var interfaces = _parentType.GetInterfaces();

            foreach (var i in interfaces)
            {
                if (i == typeof(IViewModel)) _suffix = "ViewModel";

                if (!i.IsConstructedGenericType) continue;
                if (i.GetGenericTypeDefinition() == typeof(IEntityListViewModel<>))
                {
                    _suffix = "ListViewModel";
                    _entityName = i.GenericTypeArguments[0].Name;
                    return;
                }
                if (i.GetGenericTypeDefinition() == typeof(IViewModel<>))
                {
                    _suffix = "ViewModel";
                    _entityName = i.GenericTypeArguments[0].Name;
                    return;
                }
            }
        }

        public virtual void Load(IBootContext bootstrapper)
        {
            if (!Allowed) return;

            Erp.Menu.RegisterMenu(MenuPath + "/" + Name, Header,
                OpenCommand,
                IconPath);
        }
    }

}