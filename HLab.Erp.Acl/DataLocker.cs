using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl
{
    public class DataLockerEntityDesign : IEntity<int>
    {
        private int _id;
        public object Id { get; } = 1;

        int IEntity<int>.Id
        {
            get => 1;
            set => throw new NotImplementedException();
        }

        public bool IsLoaded { get; set; } = true;
        public void OnLoaded()
        {
            throw new NotImplementedException();
        }
    }

    public class DataLockerDesign : DataLocker<DataLockerEntityDesign>, IViewModelDesign
    {
        public DataLockerDesign() : base(new DataLockerEntityDesign())
        {
        }
    }


    [Export(typeof(IDataLocker))]
    public class DataLocker<T> : N<DataLocker<T>>, IDataLocker
    where T : class,IEntity<int>
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
        private readonly T _entity;

        private EntityPersister _persister;

        [Import]
        private Func<T,EntityPersister> _getPersister;

        public DataLocker(T entity):base(false)
        {
                _persister = _getPersister(entity);

            _entity = entity;
            _entityClass = entity.GetType().Name;
            _entityId = (int)entity.Id;
            _timer = new Timer((o) => { _lock?.Heartbeat(); }, null,Timeout.Infinite,Timeout.Infinite);
            Initialize();
        }


        public bool IsActive => _isActive.Get();
        private readonly IProperty<bool> _isActive = H.Property<bool>(c => c.Default(false));

        public ICommand ActivateCommand { get; } = H.Command(c => c
            .Action(async e => await e.Activate())
        );

        public ICommand SaveCommand { get; } = H.Command(c => c
            .Action(async e => await e.Save())
        );
        public ICommand CancelCommand { get; } = H.Command(c => c
            .Action(async e => await e.Cancel())
        );

        public async Task Activate()
        {
            if (IsActive)
                return;

            var existing =
                await _db.FetchOneAsync<DataLock>(e => e.EntityClass == _entityClass && e.EntityId == _entityId);
            if (existing != null)
            {
                if ((DateTime.Now - existing.HeartbeatTime).TotalMilliseconds < HeartBeat * 2)
                {
                    _message.Set(String.Format("Object locked by {0}", existing.User.Initials));
                    return;
                }

                _db.Delete(existing);
            }

            try
            {
                _message.Set(null);
                _lock = await _db.AddAsync<DataLock>(t =>
                {
                    t.UserId = _acl.Connection.UserId;
                    t.EntityClass = _entityClass;
                    t.EntityId = _entityId;
                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _message.Set(e.Message);
                return;
            }

            _timer.Change(HeartBeat, HeartBeat);
            
        }

        public async Task Save()
        {
            _persister.Save();
            _db.Delete(_lock);
            _isActive.Set(false);
        }
        public async Task Cancel()
        {
            _db.FetchOne<T>(_entity.Id);
            _persister.Save();
            _db.Delete(_lock);
            _isActive.Set(false);
        }

        public string Message => _message.Get();
        private readonly IProperty<string> _message = H.Property<string>();

        public bool IsReadOnly => _isReadOnly.Get();
        private readonly IProperty<bool> _isReadOnly = H.Property<bool>(c => c
            .On(e => e.IsActive)
            .Set(e => !e.IsActive)
            .Default(true)
        );
        public async void Dispose()
        {
            if(IsActive)
                await Cancel().ConfigureAwait(false);
            _timer.Dispose();
        }

    }
}