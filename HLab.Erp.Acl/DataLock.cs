using System;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Acl
{
    using H = HD<DataLock>;

    public class DataLock : Entity
    {
        public DataLock() => H.Initialize(this);

        public string EntityClass        {
            get => _entityClass.Get();
            set => _entityClass.Set(value);
        }

        readonly IProperty<string> _entityClass = H.Property<string>(e => e.Default(""));
        public int? EntityId
        {
            get => _entityId.Get();
            set => _entityId.Set(value);
        }

        readonly IProperty<int?> _entityId = H.Property<int?>();

        public int? UserId
        {
            get => _userId.Get();
            set => _userId.Set(value);
        }

        readonly IProperty<int?> _userId = H.Property<int?>();

        [Ignore]
        public User User
        {
            //get => E.GetForeign<User>(() => UserId);
            get => _user.Get();
            set => UserId = value.Id;
            //set => _user.Set(value);
        }

        readonly IProperty<User> _user = H.Property<User>(c => c.Foreign(e => e.UserId));

        public string Code
        {
            get => _code.Get();
            set => _code.Set(value);
        }

        readonly IProperty<string> _code = H.Property<string>(c => c.Set(e => e.GetNewCode()));

        public DateTime StartTime
        {
            get => _startTime.Get();
            set => _startTime.Set(value);
        }

        readonly IProperty<DateTime> _startTime = H.Property<DateTime>(c => c.Set(e => DateTime.Now.ToUniversalTime()));

        public DateTime HeartbeatTime
        {
            get => _heartbeatTime.Get();
            set => _heartbeatTime.Set(value);
        }

        readonly IProperty<DateTime> _heartbeatTime = H.Property<DateTime>(c => c.Set(e => DateTime.Now.ToUniversalTime()));

        public void Heartbeat(int heartBeat)
        {
            HeartbeatTime = DateTime.Now.AddMilliseconds(heartBeat).ToUniversalTime();
        }

        const string Charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        readonly Random _rnd = new Random();

        string GetNewCode()
        {
            var result = "";
            for (int i = 0; i < 10; i++)
                result += Charset[_rnd.Next(Charset.Length)];
            return result;
        }
    }
}
