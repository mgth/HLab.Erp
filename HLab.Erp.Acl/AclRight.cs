using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;

namespace HLab.Erp.Acl
{
    public static class AclRights
    {
        public static readonly AclRight AclUserCreateRight = AclRight.Get();
        public static readonly AclRight AclGroupCreateRight = AclRight.Get();
        public static readonly AclRight AclChangePassword = AclRight.Get();
    }

    public class AclRight : Entity<AclRight>
    {
        [Import]
        public static IDataService Data { get; set; }

        public static  AclRight Get([CallerMemberName] string name = null)
        {
            return Data.GetOrAdd<AclRight>(e => e.Name == name, e => e.Name = name).Result;
        }


        public string Name { get; set; }
        public AclRight()
        {
        }
    }
}
