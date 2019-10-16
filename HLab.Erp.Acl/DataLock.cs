using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using HLab.Notify;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Acl
{

    [Export(typeof(IDataLocker))]
    public class DataLocker : N<DataLocker>, IDataLocker
    {

        private const int HeartBeat = 10000;

        [Import]
        private readonly IDataService _db;
        [Import]
        private readonly IAclService _acl;
        private DataLock _lock = null;
        private readonly string _entityClass;
        private readonly int _entityId;
        private readonly Timer _timer;
        private readonly IEntity _entity;
        public DataLocker(IEntity entity)
        {
            _entity = entity;
            _entityClass = entity.GetType().Name;
            _entityId = (int)entity.Id;
            _timer = new Timer((o) => { _lock?.Heartbeat(); }, null,Timeout.Infinite,Timeout.Infinite);
            Initialize();
        }


        public bool IsActive => _isActive.Get();
        private readonly IProperty<bool> _isActive = H.Property<bool>();

        public async Task Activate(bool value)
        {
            if (_isActive.Get() == value)
                return;

            if (value)
            {
                var existing = await _db.FetchOne<DataLock>(e => e.EntityClass == _entityClass && e.EntityId == _entityId);
                if (existing != null)
                {
                    if ((DateTime.Now - existing.HeartbeatTime).TotalMilliseconds < HeartBeat * 2) return;
                    _db.Delete(existing);
                }
                try
                {
                    _lock = await _db.Add<DataLock>(t =>
                    {
                        t.UserId = _acl.Connection.UserId;
                        t.EntityClass = _entityClass;
                        t.EntityId = _entityId;
                    });

                    //_context = _db.Get();
                    //_context.Attach(_entity);
                }
                catch (Exception e)
                {
                    return;
                }

                _timer.Change(HeartBeat, HeartBeat);
            }
            else
            {
                //_context.SaveChanges();
                _db.Delete(_lock);
            }
            _isActive.Set(value);        }


        public bool IsReadOnly => _isReadOnly.Get();
        private readonly IProperty<bool> _isReadOnly = H.Property<bool>(c => c
            .On(e => e.IsActive)
            .Set(e => !e.IsActive)
        );
        public async void Dispose()
        {
            await Activate(false);
            _timer.Dispose();
        }

    }

    public class DataLock : Entity<DataLock>
    {
        public string EntityClass        {
            get => _entityClass.Get();
            set => _entityClass.Set(value);
        }
        private readonly IProperty<string> _entityClass = H.Property<string>(e => e.Default(""));
        public int? EntityId
        {
            get => _entityId.Get();
            set => _entityId.Set(value);
        }
        private readonly IProperty<int?> _entityId = H.Property<int?>();

        public int? UserId
        {
            get => _userId.Get();
            set => _userId.Set(value);
        }
        private readonly IProperty<int?> _userId = H.Property<int?>();

        [Ignore]
        public User User
        {
            //get => E.GetForeign<User>(() => UserId);
            get => _user.Get();
            set => UserId = value.Id;
            //set => _user.Set(value);
        }
        private readonly IProperty<User> _user = H.Property<User>(c => c.Foreign(e => e.UserId));

        public string Code
        {
            get => _code.Get();
            set => _code.Set(value);
        }

        private readonly IProperty<string> _code = H.Property<string>(c => c.Set(e => e.GetNewCode()));

        public DateTime StartTime
        {
            get => _startTime.Get();
            set => _startTime.Set(value);
        }

        private readonly IProperty<DateTime> _startTime = H.Property<DateTime>(c => c.Set(e => DateTime.Now));

        public DateTime HeartbeatTime
        {
            get => _heartbeatTime.Get();
            set => _heartbeatTime.Set(value);
        }

        private readonly IProperty<DateTime> _heartbeatTime = H.Property<DateTime>(c => c.Set(e => DateTime.Now));

        public void Heartbeat()
        {
            HeartbeatTime = DateTime.Now;
        }

        private const string Charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private readonly Random _rnd = new Random();
        private string GetNewCode()
        {
            var result = "";
            for (int i = 0; i < 10; i++)
                result += Charset[_rnd.Next(Charset.Length)];
            return result;
        }
    }
}
