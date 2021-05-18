using System;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using HLab.Core.Annotations;

namespace HLab.Erp.Acl
{
    public interface IAclService : IService
    {
        Connection Connection { get; }

        bool Cancelled { get; }
        //string Login(string login, string password);
        Task<string> Login(NetworkCredential credential, bool pin = false);
        Task<User> Check(NetworkCredential credential, bool pin = false);
        string Crypt(SecureString password);


        /// <summary>
        /// Not supported yet
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        Task<AclNode> GetAclNode(object target);

        bool IsGranted(AclRight right, object grantedTo = null, object grantedOn = null);
        bool IsGranted(Action<string> setMessage, params AclRight[] rights);
        void CancelLogin();
    }
}
