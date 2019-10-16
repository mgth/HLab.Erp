using System.Net;
using System.Security;
using System.Threading.Tasks;
using HLab.Core.Annotations;

namespace HLab.Erp.Acl
{
    public interface IAclService : IService
    {
        Connection Connection { get; }
        //string Login(string login, string password);
        Task<string> Login(NetworkCredential credential, bool pin = false);
        string Crypt(SecureString password);
    }
}
