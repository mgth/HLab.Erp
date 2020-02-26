using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Acl
{
    public static class AclRights
    {
        /// <summary>
        /// Allow to create / Delete users
        /// </summary>        
        public static readonly AclRight ManageUser = AclRight.Get();
        /// <summary>
        /// Allow to change other users password
        /// </summary>        
        public static readonly AclRight ChangePassword = AclRight.Get();
        /// <summary>
        /// Allow to create / delete user profiles
        /// </summary>
        public static readonly AclRight ManageProfiles = AclRight.Get();
        /// <summary>
        /// Allow to add / remove rights for one profile
        /// </summary>
        public static readonly AclRight ManageRights = AclRight.Get();
    }

    public class AclRight : Entity<AclRight>, IListableModel
    {
        [Import]
        public static IDataService Data { get; set; }

        public static  AclRight Get([CallerMemberName] string name = null)
        {
            return Data.GetOrAddAsync<AclRight>(e => e.Name == name, e => e.Name = name).Result;
        }


        public string Name
        {
            get => _name.Get(); 
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>();

        public AclRight()
        {
        }

        [Ignore]
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c.OneWayBind(e => e.Name));

        [Ignore]
        public string IconPath => "";
    }
}
