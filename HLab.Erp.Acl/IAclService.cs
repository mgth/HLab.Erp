using System;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using HLab.Core.Annotations;

namespace HLab.Erp.Acl;

public abstract class AclServiceBase : IAclService
{
    public virtual ServiceState ServiceState => default;
    public virtual Connection? Connection => default;
    public virtual bool Cancelled => default;
    public Task<string> LoginAsync(string username, string hashedPassword, bool pin = false)
    {
        throw new NotImplementedException();
    }

    public virtual Task<string> LoginAsync(NetworkCredential credential, bool pin = false)
    {
        throw new InvalidOperationException();
    }

    public virtual Task<User?> CheckAsync(NetworkCredential credential, bool pin = false)
    {
        throw new InvalidOperationException();
    }

    public string Crypt(SecureString password)
    {
        throw new NotImplementedException();
    }

    public virtual string Crypt(string password)
    {
        throw new InvalidOperationException();
    }

    public virtual Task<AclNode> GetAclNode(object target)
    {
        throw new InvalidOperationException();
    }

    public virtual bool IsGranted(AclRight? right, object grantedTo = null, object grantedOn = null)
    {
        throw new InvalidOperationException();
    }

    public virtual bool IsGranted(Action<string> setMessage, params AclRight?[] rights)
    {
        throw new InvalidOperationException();
    }

    public virtual void CancelLogin()
    {
        throw new InvalidOperationException();
    }
}

public interface IAclService : IService
{
    Connection? Connection { get; }

    bool Cancelled { get; }
    //string Login(string login, string password);
#if DEBUG
    Task<string> LoginAsync(string username, string hashedPassword, bool pin = false);
    string Crypt(string password);
#endif
    Task<string> LoginAsync(NetworkCredential credential, bool pin = false);
    Task<User?> CheckAsync(NetworkCredential credential, bool pin = false);
    string Crypt(SecureString password);


    /// <summary>
    /// Not supported yet
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    Task<AclNode> GetAclNode(object target);

    bool IsGranted(AclRight? right, object grantedTo = null, object grantedOn = null);
    bool IsGranted(Action<string> setMessage, params AclRight?[] rights);
    void CancelLogin();
}
