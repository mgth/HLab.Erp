using System;
using System.Net;
using System.Security;
using System.Threading;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;


namespace HLab.Erp.Acl
{
    [Export(typeof(IAclService)),Singleton]
    public class AclService : IAclService
    {
        private readonly IMessageBus _msg;
        private readonly IAclHelper _acl;

        [Import] public AclService(IMessageBus msg, IAclHelper acl)
        {
            _msg = msg;
            _acl = acl;
        }

        public Connection Connection { get; private set; } = null;


        public string PinLogin(string pin)
        {
            throw new NotImplementedException();
        }
        public string Login(NetworkCredential credential,bool pin = false)
        {
            Connection connection;
            try
            {
                if (pin)
                    connection = _acl.GetConnectionWithPin(credential);
                else
                    connection = _acl.GetConnection(credential);
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

        public string Login(string login, SecureString password)
        {
            return Login(new NetworkCredential(login, password));
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
