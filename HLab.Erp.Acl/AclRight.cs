using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
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

        private static readonly ConcurrentDictionary<string, AclRight> _cache = new();
        public static AclRight Get([CallerMemberName] string name = null)
        {
            return _cache.GetOrAdd(name, e => GetFromDb(name));
        }
        private static AclRight GetFromDb(string name)
        {
            return Data?.GetOrAdd<AclRight>(
                e => e.Name == name,
                e => e.Name = name)
                ?? new AclRight { Name = name }
                ;
        }

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = HD<AclRight>.Property<string>();

        [Ignore]
        public string Caption => $"{{{Name}}}";//_caption.Get();
        private readonly IProperty<string> _caption = HD<AclRight>.Property<string>(c => c.Set(e => $"{{{e.Name}}}").On(e => e.Name).Update());

        [Ignore]
        public string IconPath => "icons/approved";

        public override string ToString() => Name;
    }
}
