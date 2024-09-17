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
    Task<User?> GetUserAsync(NetworkCredential credential);
    Task<Connection?> GetConnectionAsync(NetworkCredential credential);

    Task<User?> GetUserWithPinAsync(NetworkCredential credential);
    Task<Connection?> GetConnectionWithPinAsync(NetworkCredential credential);
    string Crypt(SecureString password);
}

public class AclHelper(IDataService db) : IAclHelper
{
    public async Task<Connection?> GetConnectionAsync(NetworkCredential credential) 
        => await GetConnectionAsync(await GetUserAsync(credential));

    public async Task<Connection?> GetConnectionWithPinAsync(NetworkCredential credential) 
        => await GetConnectionAsync(await GetUserWithPinAsync(credential));

    const string KEY = "PG8JaR0ix+GP2w0bXse/ReZugKK+Q/g/";
    const string IV = "TuG898IilQA=";

    protected IDataService Data = db;

    public async Task<User?> GetUserWithPinAsync(NetworkCredential credential)
    {
        var login = credential.UserName;
        var pin = Crypt(credential.SecurePassword);
        return await Data.FetchOneAsync<User>(u => u.Username == login && u.Pin == pin);
    }

    public virtual async Task<User?> GetUserAsync(NetworkCredential credential)
    {
        try
        {
            return await Data.FetchOneAsync<User>(u =>
                u.Username == credential.UserName && u.HashedPassword == Crypt(credential.SecurePassword));
        }
        catch (DataException ex)
        {
            throw new AclException(ex.InnerException?.Message??"",ex);
        }
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

    public string Crypt(SecureString password)
    {
        try
        {
            var symmetricAlgorithm = TripleDES.Create();
            //var symmetricAlgorithm = Aes.Create();
            symmetricAlgorithm.Key = Convert.FromBase64String(KEY);
            symmetricAlgorithm.IV = Convert.FromBase64String(IV);

            var encryptor = symmetricAlgorithm.CreateEncryptor();

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
