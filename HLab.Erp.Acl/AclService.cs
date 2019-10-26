using System;
using System.ComponentModel;
using System.Net;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;


namespace HLab.Erp.Acl
{
    public interface IAclTarget
    {
        string GetAclClass();
        int? GetAclId();
    }

    [Export(typeof(IAclService)),Singleton]
    public class AclService : IAclService
    {
        private readonly IMessageBus _msg;
        private readonly IAclHelper _acl;
        private readonly IDataService _data;

        [Import] public AclService(IMessageBus msg, IAclHelper acl, IDataService data)
        {
            _msg = msg;
            _acl = acl;
            _data = data;
        }

        public Connection Connection { get; private set; } = null;


        public string PinLogin(string pin)
        {
            throw new NotImplementedException();
        }
        public async Task<string> Login(NetworkCredential credential,bool pin = false)
        {
            Connection connection;
            try
            {
                if (pin)
                    connection = await _acl.GetConnectionWithPin(credential);
                else
                    connection = await _acl.GetConnection(credential);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            if (connection != null /*&& user.CryptedPassword == password*/)
            {
                Connection = connection;
                _msg.Publish(new UserLoggedInMessage(connection));
                return Connection.User.FirstName + " " + Connection.User.Name + " Connecté.";
            }
            Thread.Sleep(5000);
            return "Identifiant ou mot de passe incorrect.";
        }

        public string Crypt(SecureString password) => _acl.Crypt(password);

        public async Task<string> Login(string login, SecureString password)
        {
            return await Login(new NetworkCredential(login, password));
        }

        public async Task<AclNode> GetAclNode(object target)
        {
            string aclClass;
            int? id;

            if (target is IAclTarget at)
            {
                aclClass = at.GetAclClass();
                id = at.GetAclId();
            }
            else if (target is IEntity<int?> e)
            {
                aclClass = e.GetType().Name;
                id = e.Id;
            }
            else return null;


            var node = await _data.GetOrAdd<AclNode>(
                e => e.TargetClass == aclClass && e.TargetId == id,
                e =>
                {
                    e.TargetClass = aclClass;
                    e.TargetId = id;
                }).ConfigureAwait(false);
            return node;
        }

        public async Task<bool> IsGranted(AclRight right, object grantedTo, object grantedOn = null)
        {
            var toNode = await GetAclNode(grantedTo).ConfigureAwait(false);
            var onNode = await GetAclNode(grantedOn).ConfigureAwait(false);

            return await toNode.IsGranted(right, onNode).ConfigureAwait(false);
        }

        //private ConcurrentDictionary<string,DataLock> _locks = new ConcurrentDictionary<string,DataLock>();


        //public DataLock GetLock(string name)
        //{
        //    var lck = _db.FetchOne<DataLock>(name);
        //    if (lck != null) return null;
        //    return lck;
        //}
    }
}
