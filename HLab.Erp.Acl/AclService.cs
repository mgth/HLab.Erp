using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes;
using HLab.Core.Annotations;
using HLab.Erp.Data;
using NPoco;


namespace HLab.Erp.Acl
{
    public class AclException : Exception
    {
        public AclException(String message, Exception inner) : base(message, inner)
        {

        }
    }
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

        public AclService(IMessageBus msg, IAclHelper acl, IDataService data)
        {
            _msg = msg;
            _acl = acl;
            _data = data;
        }

        public Connection Connection { get; private set; } = null;

        public bool Cancelled { get; private set; } = false;

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
                await PopulateRightsAsync();
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


            var node = await _data.GetOrAddAsync<AclNode>(
                e => e.TargetClass == aclClass && e.TargetId == id,
                e =>
                {
                    e.TargetClass = aclClass;
                    e.TargetId = id;
                }).ConfigureAwait(false);
            return node;
        }

        public List<AclRight> CurrentRights { get; private set; }

        public bool IsGranted(AclRight right, object grantedTo = null, object grantedOn = null)
        {
            if(Connection==null) return false;
            if(Connection.User.Login=="admin") return true;
            if (right == null) return true;
            return CurrentRights.Contains(right);
        }
        public bool IsGranted(Action<string> setMessage,params AclRight[] rights)
        {
            if(Connection==null) return false;
            if(Connection.User.Login=="admin") return true;
            if (rights.Length == 0) return true;
            foreach (var right in rights)
            {
                if (CurrentRights.Contains(right)) return true;
            }
            foreach (var right in rights)
            {
                setMessage($"{{Need right}} : {right.Caption}");
            }
            return false;
        }

        public void CancelLogin()
        {
            Cancelled = true;
        }

        private async Task PopulateRightsAsync()
        {
            List<AclRight> rights = new List<AclRight>();

            var userProfiles = _data.FetchWhereAsync<UserProfile>(e => e.UserId == Connection.UserId, e => e.Id);
            await foreach (var userProfile in userProfiles)
            {
                var profileRights = _data.FetchWhereAsync<AclRightProfile>(e => e.ProfileId == userProfile.ProfileId, e => e.Id);
                await foreach(var rightProfile in profileRights)
                {
                    if(rights.Contains(rightProfile.AclRight))
                        continue;

                    rights.Add(rightProfile.AclRight);
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

            return await toNode.IsGrantedAsync(right, onNode).ConfigureAwait(false);
        }

        public ServiceState ServiceState { get; internal set; } = ServiceState.Available;

        //private ConcurrentDictionary<string,DataLock> _locks = new ConcurrentDictionary<string,DataLock>();


        //public DataLock GetLock(string name)
        //{
        //    var lck = _db.FetchOne<DataLock>(name);
        //    if (lck != null) return null;
        //    return lck;
        //}
    }
}
