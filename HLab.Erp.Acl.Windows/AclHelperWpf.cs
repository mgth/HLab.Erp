using System.DirectoryServices.AccountManagement;
using System.Net;
using HLab.Erp.Data;

namespace HLab.Erp.Acl.Windows;

public class AclHelperWindows(IDataService db) : AclHelper(db)
{
    public override async Task<User> GetUser(NetworkCredential credential)
    {
        try
        {
            var user = await Data.FetchOneAsync<User>(u => u.Username == credential.UserName).ConfigureAwait(false);
            if (user != null && !string.IsNullOrWhiteSpace(user.Domain))
            {
                try
                {
                    using var context = new PrincipalContext(ContextType.Domain, user.Domain);
                    var valid = context.ValidateCredentials(credential.UserName, credential.Password);
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

        return await base.GetUser(credential).ConfigureAwait(false);
    }
}
