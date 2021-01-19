using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using Nito.AsyncEx;

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
    public class DataLocker<T> : NotifierBase, IDataLocker
    where T : class,IEntity<int>
    {
        private const int HeartBeat = 10000;

        [Import] private readonly IDataService _db;
        [Import] private readonly IAclService _acl;
        private DataLock _lock = null;
        private readonly string _entityClass;
        private readonly int _entityId;
        private readonly Timer _timer;
        private readonly T _entity;

        public EntityPersister<T> Persister {
            get => _persister.Get(); 
            private set => _persister.Set(value); 
        }
        private readonly IProperty<EntityPersister<T>> _persister = H<DataLocker<T>>.Property<EntityPersister<T>>();

        private EntityPersister<DataLock> _lockPersister;

        [Import] private Func<T,EntityPersister<T>> _getPersister;
        [Import] private Func<DataLock,EntityPersister<DataLock>> _getLockPersister;

        public DataLocker(T entity)
        {
            _entity = entity;
            _entityClass = entity.GetType().Name;
            _entityId = (int)entity.Id;
            _timer = new Timer((o) => {
                _lock?.Heartbeat(HeartBeat);
                try
                {
                    Message = null;
                    if (_lockPersister.Save())
                    {
                        IsConnected = true;
                        return;
                    }
                    else DirtyCancel();
                }
                catch (DataException ex)
                {
                    Message = "{Disconnected}";
                    IsConnected = false;
                }
            }, null,Timeout.Infinite,Timeout.Infinite);

            H<DataLocker<T>>.Initialize(this);

            Persister = _getPersister(entity);

            // default to edit mode when entity is new and not saved.
            if (entity.Id < 0)
                IsActive = true;
        }


        public bool IsActive
        {
            get => _isActive.Get();
            private set => _isActive.Set(value);
        }
        private readonly IProperty<bool> _isActive = H<DataLocker<T>>.Property<bool>(c => c.Default(false));
        public bool IsEnabled
        {
            get => _isEnabled.Get();
            set => _isEnabled.Set(value);
        }
        private readonly IProperty<bool> _isEnabled = H<DataLocker<T>>.Property<bool>(c => c.Default(false));
        public bool IsConnected
        {
            get => _isConnected.Get();
            set => _isConnected.Set(value);
        }
        private readonly IProperty<bool> _isConnected = H<DataLocker<T>>.Property<bool>(c => c.Default(false));

        public ICommand ActivateCommand { get; } = H<DataLocker<T>>.Command(c => c
            .Action(async e => await e.ActivateAsync().ConfigureAwait(false))
        );

        public ICommand SaveCommand { get; } = H<DataLocker<T>>.Command(c => c
            .CanExecute(e => e.Persister.IsDirty)
            .Action(async e => await e.SaveAsync().ConfigureAwait(false))
            .On(e => e.Persister.IsDirty).CheckCanExecute()
        );

        public ICommand CancelCommand { get; } = H<DataLocker<T>>.Command(c => c
            .Action(async e => await e.CancelAsync().ConfigureAwait(false))
        );

        public async Task ActivateAsync()
        {
            if (IsActive)
                return;

            var existing =
                await _db.FetchOneAsync<DataLock>(e => e.EntityClass == _entityClass && e.EntityId == _entityId).ConfigureAwait(true);

            if (existing != null)
            {
                //Already locked
                if ((DateTime.Now - existing.HeartbeatTime).TotalMilliseconds < HeartBeat)
                {
                    Message = $"{{Object locked by}} {existing.User.Initials}";
                    return;
                }

                _db.Delete(existing);
            }

            if(_entityId >= 0)
            {
                try
                {
                    //Clear error message
                    Message = null;

                    //Generate data lock token
                    _lock = await _db.AddAsync<DataLock>(t =>
                    {
                        t.UserId = _acl.Connection.UserId;
                        t.EntityClass = _entityClass;
                        t.EntityId = _entityId;
                    }).ConfigureAwait(true);

                    // Get a persister on lock
                    _lockPersister = _getLockPersister(_lock);

                    //Reload entity to be sure no changes occured before locking
                    await _db.ReFetchOneAsync(_entity).ConfigureAwait(true);
                }
                catch (Exception e)
                {
                    Message = e.Message;
                    if(_lock!=null) _db.Delete(_lock);
                    _lock = null;

                    _lockPersister=null;

                    return;
                }

                _timer.Change(HeartBeat, HeartBeat);
            }

            IsActive = true;
        }

        [Import] private Func<IDataTransaction,IAuditTrailProvider> _getAudit;

        public Action<T> BeforeSavingAction { get; set; }

        public async Task<bool> SaveAsync()
        {
            BeforeSavingAction?.Invoke(_entity);

            var transaction = _db.GetTransaction();
            try
            {
                Message = null;

                var log = Persister.ToString();

                var action = _entityId < 0 ? "Create" : "Update";

                //TODO : add AclRight needed to do the action
                if (_getAudit(transaction).Audit(action, null, log, _entity))
                {
                    await Persister.SaveAsync(transaction);
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);

                    if (_lock != null)
                        await transaction.DeleteAsync(_lock);

                    IsActive = false;
                    _lock = null;

                    transaction.Done();
                    return true;
                }
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
            finally
            {
                transaction?.Dispose();
            }

            return false;
        }

        private readonly AsyncReaderWriterLock _cancelLock = new AsyncReaderWriterLock();

        private void DirtyCancel()
        {
            using(_cancelLock.WriterLock())
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);

                try
                {
                    _db.ReFetchOneAsync(_entity).ConfigureAwait(true);
                }
                catch{}

                _lock = null;
                Persister.Reset();
                IsActive = false;
            }
        }


        public async Task CancelAsync()
        {
            try
            {
                var lck = await _cancelLock.WriterLockAsync();
                using (lck)
                {
                    Message = null;
                    if (_lock != null)
                    {
                        _timer.Change(Timeout.Infinite, Timeout.Infinite);
                        if (_db.Delete(_lock))
                        {
                            _lock = null;
                            await _db.ReFetchOneAsync(_entity).ConfigureAwait(true);
                            Persister.Reset();
                            IsActive = false;
                        }
                    }

                    else IsActive = false;
                }
            }
            catch (DataException e)
            {
                Message = e.Message;
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
        }

        public string Message
        {
            get => _message.Get();
            private set => _message.Set(value);
        }

        private readonly IProperty<string> _message = H<DataLocker<T>>.Property<string>();

        public bool IsReadOnly => _isReadOnly.Get();
        private readonly IProperty<bool> _isReadOnly = H<DataLocker<T>>.Property<bool>(c => c
            .On(e => e.IsActive)
            .Set(e => !e.IsActive)
            .Default(true)
        );
        public async void Dispose()
        {
            if(IsActive)
                await CancelAsync().ConfigureAwait(false);
            await _timer.DisposeAsync();
        }

    }
}