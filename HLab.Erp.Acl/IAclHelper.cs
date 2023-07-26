using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HLab.Erp.Data;


namespace HLab.Erp.Acl;

public interface IAclHelper
{
    Task<User> GetUser(NetworkCredential credential);
    Task<Connection> GetConnection(NetworkCredential credential);

    Task<User> GetUserWithPin(NetworkCredential credential);
    Task<Connection> GetConnectionWithPin(NetworkCredential credential);
    string Crypt(SecureString password);
}

public class AclHelper : IAclHelper
{
    public async Task<Connection> GetConnection(NetworkCredential credential) => await GetConnection(await GetUser(credential).ConfigureAwait(false)).ConfigureAwait(false);

    public async Task<Connection> GetConnectionWithPin(NetworkCredential credential) => await GetConnection(await GetUserWithPin(credential).ConfigureAwait(false)).ConfigureAwait(false);


    const string Cle = "PG8JaR0ix+GP2w0bXse/ReZugKK+Q/g/";
    const string Iv = "TuG898IilQA=";

    protected IDataService Data;

    public AclHelper(IDataService db)
    {
        Data = db;
    }

    public async Task<User> GetUserWithPin(NetworkCredential credential)
    {
        var login = credential.UserName;
        var pin = Crypt(credential.SecurePassword);
        return await Data.FetchOneAsync<User>(u => u.Login == login && u.Pin == pin);
    }

    public virtual async Task<User> GetUser(NetworkCredential credential)
    {
        try
        {
            return await Data.FetchOneAsync<User>(u =>
                u.Login == credential.UserName && u.HashedPassword == Crypt(credential.SecurePassword));
        }
        catch (DataException ex)
        {
            throw new AclException(ex.InnerException?.Message,ex);
        }
    }

    async Task<Connection> GetConnection(User user)
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

    public string Crypt(SecureString password)
    {
        try
        {
            var symmetricAlgorithm = new TripleDESCryptoServiceProvider();
            var encryptor = symmetricAlgorithm.CreateEncryptor(Convert.FromBase64String(Cle), Convert.FromBase64String(Iv));

            var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            var valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(password);
                var p = Marshal.PtrToStringUni(valuePtr);
                if (p == null) return "";
                var bytes = Encoding.UTF8.GetBytes(p);
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();
                cryptoStream.Close();

                return Convert.ToBase64String(memoryStream.ToArray());
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
        catch
        {
            return "";
        }
    }

}
