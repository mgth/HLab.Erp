using System.DirectoryServices.AccountManagement;
using System.Net;
using HLab.Core;
using HLab.Erp.Data;

namespace HLab.Erp.Acl.Windows;

public class AclHelperWindows(ICryptService crypt, IDataService db) : AclHelper(db,crypt)
{
    public override async Task<User?> GetUserAsync(NetworkCredential credential)
    {
        var username = credential.UserName;
        var password = credential.Password;

        User? user = null;

        try
        {
            user = await Data.FetchOneAsync<User>(u => u.Username == username);
            if (user != null && !string.IsNullOrWhiteSpace(user.Domain))
            {
                try
                {
                    using var context = new PrincipalContext(ContextType.Domain, user.Domain);
                    var valid = context.ValidateCredentials(username, password);
                    if (valid) return user;
                }
                catch
                {
                }
            }
        }
        catch (DataException ex)
        {
            throw new AclException(ex.InnerException?.Message, ex);
        }

        user = await base.GetUserAsync(credential);
        return user;
    }
}
