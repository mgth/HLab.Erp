using System.DirectoryServices.AccountManagement;
using System.Net;
using HLab.Erp.Data;

namespace HLab.Erp.Acl.Windows;

public class AclHelperWindows(IDataService db) : AclHelper(db)
{
    public override async Task<User?> GetUserAsync(NetworkCredential credential)
    {
        var username = credential.UserName;
        var password = credential.Password;

        try
        {
            var user = await Data.FetchOneAsync<User>(u => u.Username == username);
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

        return await base.GetUserAsync(credential);
    }
}
