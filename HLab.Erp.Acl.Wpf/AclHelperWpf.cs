using System.DirectoryServices.AccountManagement;
using System.Net;
using System.Threading.Tasks;
using HLab.Erp.Data;

namespace HLab.Erp.Acl
{
    public class AclHelperWpf : AclHelper
    {
        public override async Task<User> GetUser(NetworkCredential credential)
        {
            try
            {
                var user = await Data.FetchOneAsync<User>(u => u.Login == credential.UserName).ConfigureAwait(false);
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
            catch(DataException ex)
            {
                throw new AclException(ex.InnerException?.Message,ex);
            }

            return await base.GetUser(credential).ConfigureAwait(false);
        }
    }
}
