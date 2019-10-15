using System.DirectoryServices.AccountManagement;
using System.Net;
using HLab.DependencyInjection.Annotations;

namespace HLab.Erp.Acl.Wpf
{
    [Export(typeof(IAclService)), Singleton]
    public class AclServiceWpf : AclService
    {
        public override User GetUser(NetworkCredential credential)
        {
            bool valid = false;
                try
                {
                    using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
                    {
                        valid = context.ValidateCredentials(credential.UserName, credential.Password);
                    }
                }
                catch
                {
                    valid = false;
                }

            if (valid) return _db.FetchOne<User>(u => u.Login == credential.UserName);
            else return base.GetUser(credential);

            return null;

        }
    }
}
