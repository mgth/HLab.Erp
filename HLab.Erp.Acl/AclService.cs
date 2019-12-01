using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        public async Task<User> Check(NetworkCredential credential, bool pin = false)
        {
            try
            {
                if (pin)
                    return await _acl.GetUserWithPin(credential).ConfigureAwait(true);
                else
                    return await _acl.GetUser(credential).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<string> Login(NetworkCredential credential,bool pin = false)
        {
            Connection connection;
            try
            {
                if (pin)
                    connection = await _acl.GetConnectionWithPin(credential).ConfigureAwait(true);
                else
                    connection = await _acl.GetConnection(credential).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            if (connection != null /*&& user.CryptedPassword == password*/)
            {
                Connection = connection;
                await PopulateRights();
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

        public List<AclRight> CurrentRights;

        public bool IsGranted(AclRight right, object grantedTo = null, object grantedOn = null)
        {
            if(Connection.User.Login=="admin") return true;
            return CurrentRights.Contains(right);
        }

        private async Task PopulateRights()
        {
            List<AclRight> rights = new List<AclRight>();

            var userId = Connection.UserId;
            var profiles = await _data.FetchWhereAsync<UserProfile>(e => e.UserId == userId, e => e.Id);
            foreach (var profile in profiles)
            {
                var profileRights = await _data.FetchWhereAsync<AclRightProfile>(e => e.ProfileId == profile.Id, e => e.Id);
                foreach(var rightProfile in profileRights)
                {
                    if(rights.Any(r => r.Id == rightProfile.AclRightId)) continue;
                    var right = await _data.FetchOne<AclRight>(rightProfile.AclRightId.Value);

                    rights.Add(right);
                }
            }

            CurrentRights = rights;
        }

        public async Task<bool> IsGrantedV2(AclRight right, object grantedTo, object grantedOn = null)
        {
            var toNode = await GetAclNode(grantedTo).ConfigureAwait(false);
            var onNode = await GetAclNode(grantedOn).ConfigureAwait(false);

            //TODO : should it be false or true (or exception) 
            if (toNode == null) return false;
            if (onNode == null) return false;

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
