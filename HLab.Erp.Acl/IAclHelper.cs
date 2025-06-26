using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HLab.Core;
using HLab.Core.Annotations;
using HLab.Erp.Data;

namespace HLab.Erp.Acl;

public interface IAclHelper
{
   Task<User?> GetUserAsync(NetworkCredential credential);

#if DEBUG
    Task<User?> GetUserWithEncryptedPasswordAsync(string username, string hashedPassword);
    Task<Connection?> GetConnectionAsync(string username, string hashedPassword);
#endif

    Task<Connection?> GetConnectionAsync(NetworkCredential credential);

    Task<User?> GetUserWithPinAsync(NetworkCredential credential);
    Task<Connection?> GetConnectionWithPinAsync(NetworkCredential credential);

    string Crypt(string password);
    string Crypt(SecureString password);
}

public class AclHelper(IDataService db, ICryptService crypt) : IAclHelper
{

    public async Task<Connection?> GetConnectionAsync(NetworkCredential credential) 
        => await GetConnectionAsync(await GetUserAsync(credential));

    public async Task<Connection?> GetConnectionWithPinAsync(NetworkCredential credential) 
        => await GetConnectionAsync(await GetUserWithPinAsync(credential));


   protected IDataService Data = db;


   public async Task<User?> GetUserWithPinAsync(NetworkCredential credential)
    {
        var login = credential.UserName;
        var valuePtr = Marshal.SecureStringToGlobalAllocUnicode(credential.SecurePassword);
        var pin = Crypt(Marshal.PtrToStringUni(valuePtr));

        return await Data.FetchOneAsync<User>(u => u.Username == login && u.Pin == pin);
    }

    #if DEBUG
    public async Task<Connection?> GetConnectionAsync(string username, string hashedPassword)
        => await GetConnectionAsync(await GetUserWithEncryptedPasswordAsync(username, hashedPassword));

    
    #endif
    public virtual async Task<User?> GetUserWithEncryptedPasswordAsync(string username, string hashedPassword)
    {
        try
        {
            var user = await Data.FetchOneAsync<User>(u =>
                u.Username == username && u.HashedPassword == hashedPassword);

            return user;
        }
        catch (DataException ex)
        {
            throw new AclException(ex.InnerException?.Message??"",ex);
        }

    }
    public virtual async Task<User?> GetUserAsync(string username, string password)
    {
        return await GetUserWithEncryptedPasswordAsync(username,Crypt(password));
    }


    public virtual async Task<User?> GetUserAsync(NetworkCredential credential)
    {
        var valuePtr = Marshal.SecureStringToGlobalAllocUnicode(credential.SecurePassword);
        var password = Marshal.PtrToStringUni(valuePtr);

        return await GetUserAsync(credential.UserName, password);
    }

    async Task<Connection?> GetConnectionAsync(User? user)
    {
        if (user == null) return null;

        return await Data.AddAsync<Connection>(c =>
        {
            try
            {
                var identity = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\');
                c.Account = identity.Length > 1 ? identity[1] : identity[0];
                c.Domain = identity.Length > 1 ? identity[0] : "";
            }
            catch (NotSupportedException)
            {
            }

            c.Exe = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "Unknown";
            c.Framework = Environment.Version.ToString();
            c.Workstation = Environment.MachineName;
            c.Notify = 0;
            c.Os = Environment.OSVersion.VersionString;
            c.UserId = user.Id;
            c.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            c.X64 = Environment.Is64BitProcess;
        }).ConfigureAwait(false);
    }

    public string Crypt(string password) => crypt.Crypt(password);

   public string Crypt(SecureString securePassword)
    {
        var valuePtr = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
        var password = Marshal.PtrToStringUni(valuePtr);
        return Crypt(password);
    }
}
