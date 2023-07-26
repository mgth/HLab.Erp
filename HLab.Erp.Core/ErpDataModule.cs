using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Base.Extensions;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using ReactiveUI;

namespace HLab.Erp.Core
{
    public abstract class ParamBootloader : NestedBootloader
    {
        public override string MenuPath => "param";
    }

    public abstract class NestedBootloader : ReactiveObject, IBootloader
    {
        public IDocumentService Docs { get; set; } 
        public IMenuService Menu { get; set; } 
        protected NestedBootloader(string menuPath = "data")
        {
            MenuPath = menuPath;
            GetEntityName();

            OpenCommand = ReactiveCommand.CreateFromTask(Open);
        }

        public virtual string Caption => Name.FromCamelCase();
        public virtual string Name => _parentType.Name.BeforeSuffix(_suffix);
        public virtual string Header => $"{{{Caption}}}";
        public virtual string IconPath
        {
            get
            {
                var name = _entityName;

                if (name.EndsWith("Class"))
                {
                    name = name[..^5];
                    return $"Icons/Entities/{name}|Icons/Class";
                }

                return $"Icons/Entities/{name}";
            }
        }

        public virtual string MenuPath { get; }
        public virtual bool Allowed => true;


        public ICommand OpenCommand { get; }

        Task Open() => Docs.OpenDocumentAsync(GetType().DeclaringType);

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

                var t = i.GetGenericTypeDefinition();

                if (t == typeof(IEntityListViewModel<>))
                {
                    _suffix = "ListViewModel";
                    _entityName = i.GenericTypeArguments[0].Name;
                    return;
                }
                else if (t == typeof(IViewModel<>))
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

            Menu.RegisterMenu($"{MenuPath}/{Name.ToLower()}" , Header,
                OpenCommand,
                IconPath);
        }
    }

}