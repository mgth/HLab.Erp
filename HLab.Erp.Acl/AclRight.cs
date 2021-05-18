using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Acl
{
    public class AclRight : Entity, IListableModel
    {
        public AclRight() => HD<AclRight>.Initialize(this);

        public static IDataService Data { get; set; }

        public static  AclRight Get([CallerMemberName] string name = null)
        {
            return Data.GetOrAdd<AclRight>(e => e.Name == name, e => e.Name = name);
        }

        public string Name
        {
            get => _name.Get(); 
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = HD<AclRight>.Property<string>();

        [Ignore]
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = HD<AclRight>.Property<string>(c => c.Bind(e => e.Name));

        [Ignore]
        public string IconPath => "";

        public override string ToString() => Name;
    }
}
