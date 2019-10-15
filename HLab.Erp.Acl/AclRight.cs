using System.Runtime.CompilerServices;

namespace HLab.Erp.Acl
{
    public static class AclRights
    {
        public static readonly AclRight AclUserCreateRight = new AclRight();
        public static readonly AclRight AclGroupCreateRight = new AclRight();
    }

    public class AclRight
    {
        public string Name { get; }
        public AclRight([CallerMemberName] string name = null)
        {
            Name = name;
        }
    }
}
