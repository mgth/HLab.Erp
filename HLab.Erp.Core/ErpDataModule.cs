using System.Windows.Input;
using HLab.Base.Extensions;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core
{
    public abstract class ErpParamModule<TList> : ErpDataModule<TList>
    {
        protected override string MenuPath => "param";
    }
    public abstract class ErpDataModule<TList> : NotifierBase, IBootloader 
    {
        [Import]
        private readonly IErpServices _erp;

        protected ErpDataModule() => H<ErpDataModule<TList>>.Initialize(this);

        public ICommand OpenCommand { get; } = H<ErpDataModule<TList>>.Command(c => c
            .Action(e => e._erp.Docs.OpenDocumentAsync(typeof(TList)))
            .CanExecute(e => true)
        );

        private string Caption => Name.FromCamelCase();
        private string Name => GetType().Name.BeforeSuffix("DataModule");

        private string EntityName()
        {
            var interfaces = typeof(TList).GetInterfaces();

            foreach (var i in interfaces)
            {
                if (i.IsConstructedGenericType)
                {
                    if (i.GetGenericTypeDefinition() == typeof(IEntityListViewModel<>))
                    {
                        return i.GenericTypeArguments[0].Name;
                    }
                }
            }

            return "";
        }

        protected virtual string Header => "{" + Caption + "}";
        protected virtual string IconPath => "Icons/Entities/" + EntityName();

        protected virtual string MenuPath => "data";
        public virtual void Load(IBootContext b) => _erp.Menu.RegisterMenu(MenuPath + "/" + Name, Header,
                OpenCommand,
                IconPath);
        
    }
}