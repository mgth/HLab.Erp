using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using HLab.Base.Extensions;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf.Entities
{
    public abstract class ErpDataModule<T,TList> : N<T>, IBootloader 
        where T:ErpDataModule<T,TList>
    {
        [Import]
        private readonly IErpServices _erp;

        public ICommand OpenCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(TList))
        ).CanExecute(e => true));

        private string Name => GetType().Name.BeforeSuffix("DataModule").FromCamelCase();

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

        protected virtual string Header => "{" + Name + "}";
        protected virtual string IconPath => "Icons/Entities/" + EntityName();

        public virtual void Load(IBootContext b) => _erp.Menu.RegisterMenu("data/"+ Name, Header,
                OpenCommand,
                IconPath);
        
    }
}