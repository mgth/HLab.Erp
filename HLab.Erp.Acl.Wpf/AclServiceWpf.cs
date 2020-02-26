using System.DirectoryServices.AccountManagement;
using System.Net;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;

namespace HLab.Erp.Acl
{
    [Export(typeof(IAclHelper)), Singleton]
    public class AclHelperWpf : AclHelper
    {
        public override async Task<User> GetUser(NetworkCredential credential)
        {
            var user =  await Data.FetchOneAsync<User>(u => u.Login == credential.UserName).ConfigureAwait(false);
            if(user!=null && !string.IsNullOrWhiteSpace(user.Domain))
            {
                bool valid = false;
                try
                {
                    using PrincipalContext context = new PrincipalContext(ContextType.Domain, user.Domain);
                    valid = context.ValidateCredentials(credential.UserName, credential.Password);
                }
                catch
                {
                    valid = false;
                }

                if (valid) return user;
            }
            return await base.GetUser(credential).ConfigureAwait(false);
        }
    }
}
