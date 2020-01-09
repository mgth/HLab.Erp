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
#if !DEBUG
            bool valid = false;
                try
                {
                    using (PrincipalContext context = new PrincipalContext(ContextType.Domain,"DCM"))
                    {
                        valid = context.ValidateCredentials(credential.UserName, credential.Password);
                    }
                }
                catch
                {
                    valid = false;
                }

            if (valid) return await Data.FetchOneAsync<User>(u => u.Login == credential.UserName).ConfigureAwait(false);
#endif            
            return await base.GetUser(credential).ConfigureAwait(false);
        }
    }
}
