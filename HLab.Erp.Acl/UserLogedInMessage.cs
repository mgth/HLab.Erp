namespace HLab.Erp.Acl
{
    public class UserLoggedInMessage
    {
        public UserLoggedInMessage(Connection connection)
        {
            Connection = connection;
        }

        public Connection Connection { get; }
    }
}
