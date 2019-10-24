using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl
{
    public class DataLockerEntityDesign : IEntity
    {
        public object Id { get; } = 1;
        public bool IsLoaded { get; set; } = true;
        public void OnLoaded()
        {
            throw new NotImplementedException();
        }
    }

    public class DataLockerDesign : DataLocker, IViewModelDesign
    {
        public DataLockerDesign() : base(new DataLockerEntityDesign())
        {
        }
    }

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
        public DataLocker(IEntity entity):base(false)
        {
            _entity = entity;
            _entityClass = entity.GetType().Name;
            _entityId = (int)entity.Id;
            _timer = new Timer((o) => { _lock?.Heartbeat(); }, null,Timeout.Infinite,Timeout.Infinite);
            Initialize();
        }


        public bool IsActive => _isActive.Get();
        private readonly IProperty<bool> _isActive = H.Property<bool>(c => c.Default(false));

        public ICommand ActivateCommand { get; } = H.Command(c => c
            .Action(async e => await e.Activate(true))
        );

        public ICommand DeactivateCommand { get; } = H.Command(c => c
            .Action(async e => await e.Activate(false))
        );

        public async Task Activate(bool value)
        {
            if (IsActive == value)
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
            .Default(true)
        );
        public async void Dispose()
        {
            await Activate(false);
            _timer.Dispose();
        }

    }
}