using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using Nito.AsyncEx;


namespace HLab.Erp.Acl
{

    public class DataLocker<T> : NotifierBase, IDataLocker<T>
    where T : class, IEntity<int>
    {
        private const int HeartBeat = 10000;

        private readonly IDataService _data;
        private readonly IAclService _acl;
        private readonly string _entityClass;
        private readonly int _entityId;
        private readonly Timer _timer;
        private readonly T _entity;

        private DataLock _lock;
        private List<IDataLocker> _dependencies = new List<IDataLocker>();

        public void AddDependencyLocker(params IDataLocker[] lockers)
        {
            foreach(var locker in lockers)
                _dependencies.Add(locker);
        }

        public EntityPersister<T> Persister
        {
            get => _persister.Get();
            private set => _persister.Set(value);
        }

        private readonly IProperty<EntityPersister<T>> _persister = H<DataLocker<T>>.Property<EntityPersister<T>>();

        private EntityPersister<DataLock> _lockPersister;

        private Func<T, EntityPersister<T>> _getPersister;
        private readonly Func<DataLock, EntityPersister<DataLock>> _getLockPersister;
        private readonly Func<IDataTransaction, IAuditTrailProvider> _getAudit;

        public DataLocker(
            T entity,

            IDataService db,
            IAclService acl,
            Func<T, EntityPersister<T>> getPersister,
            Func<DataLock, EntityPersister<DataLock>> getLockPersister,
            Func<IDataTransaction, IAuditTrailProvider> getAudit
        )
        {
            _entity = entity;
            _entityClass = entity.GetType().Name;
            _entityId = (int)entity.Id;
            _data = db;
            _acl = acl;
            _getPersister = getPersister;
            _getLockPersister = getLockPersister;
            _getAudit = getAudit;

            H<DataLocker<T>>.Initialize(this);

            _timer = new Timer(async (o) =>
            {
                _lock?.Heartbeat(HeartBeat);
                try
                {
                    Message = null;
                    if (await _lockPersister.SaveAsync())
                    {
                        IsConnected = true;
                        return;
                    }
                    else await DirtyCancelAsync();
                }
                catch (DataException ex)
                {
                    Message = "{Disconnected}";
                    IsConnected = false;
                }
            }, null, Timeout.Infinite, Timeout.Infinite);

            Persister = _getPersister?.Invoke(_entity);

            // default to edit mode when entity is new and not saved.
            if (_entity.Id < 0)
                IsActive = true;
        }

        /// <summary>
        /// True when locker is on
        /// </summary>
        public bool IsActive
        {
            get => _isActive.Get();
            private set => _isActive.Set(value);
        }
        private readonly IProperty<bool> _isActive = H<DataLocker<T>>.Property<bool>();

        /// <summary>
        /// True when locking allowed
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled.Get();
            set => _isEnabled.Set(value);
        }
        private readonly IProperty<bool> _isEnabled = H<DataLocker<T>>.Property<bool>(c => c.Default(false));

        /// <summary>
        /// True when database is responding
        /// </summary>
        public bool IsConnected
        {
            get => _isConnected.Get();
            set => _isConnected.Set(value);
        }
        private readonly IProperty<bool> _isConnected = H<DataLocker<T>>.Property<bool>(c => c.Default(false));

        /// <summary>
        /// Command to activate locker
        /// </summary>
        public ICommand ActivateCommand { get; } = H<DataLocker<T>>.Command(c => c
            .Action(async e => await e.ActivateAsync().ConfigureAwait(false))
        );

        /// <summary>
        /// Command to save data to database
        /// </summary>
        public ICommand SaveCommand { get; } = H<DataLocker<T>>.Command(c => c
            .CanExecute(e => e.Persister.IsDirty)
            .Action(async e =>
            {
                string caption = "";
                string iconPath = "";

                if (e._entity is IListableModel lm)
                {
                    caption = lm.Caption;
                    iconPath = lm.IconPath;
                }


                await e.SaveAsync(caption, iconPath, false, false).ConfigureAwait(false);
            })
            .On(e => e.Persister.IsDirty).CheckCanExecute()
        );

        /// <summary>
        /// Cancel changes and reload data from database 
        /// </summary>
        public ICommand CancelCommand { get; } = H<DataLocker<T>>.Command(c => c
            .Action(async e => await e.CancelAsync().ConfigureAwait(false))
        );


        private async Task CancelDependenciesAsync()
        {
            foreach (var locker in _dependencies)
            {
                if(locker.IsActive)
                    await locker.CancelAsync();
            }
        }


        /// <summary>
        /// Try to activate locker
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ActivateAsync()
        {
            if (IsActive)
                return true;

            var existing =
                await _data.FetchOneAsync<DataLock>(e => e.EntityClass == _entityClass && e.EntityId == _entityId).ConfigureAwait(true);

            if (existing != null)
            {
                //Already locked
                if ((DateTime.Now - existing.HeartbeatTime).TotalMilliseconds < HeartBeat)
                {
                    Message = $"{{Object locked by}} {existing.User.Initials}";
                    return false;
                }

                _data.Delete(existing);
            }

            if (_entityId >= 0)
            {
                try
                {
                    //Clear error message
                    Message = null;

                    //First lock all dependencies
                    foreach (var locker in _dependencies)
                    {
                        if (!await locker.ActivateAsync())
                        {
                            Message = locker.Message;
                            await CancelDependenciesAsync();
                            return false;
                        }
                    }

                    //Generate data lock token
                    _lock = await _data.AddAsync<DataLock>(t =>
                    {
                        t.UserId = _acl.Connection.UserId;
                        t.EntityClass = _entityClass;
                        t.EntityId = _entityId;
                    }).ConfigureAwait(true);

                    // Get a persister on lock
                    _lockPersister = _getLockPersister(_lock);

                    //Reload entity to be sure no changes occured before locking
                    await _data.ReFetchOneAsync(_entity).ConfigureAwait(true);
                }
                catch (Exception e)
                {
                    Message = e.Message;
                    if (_lock != null) _data.Delete(_lock);
                    _lock = null;

                    _lockPersister = null;

                    return false;
                }

                _timer.Change(HeartBeat, HeartBeat);
            }

            IsActive = true;
            return true;
        }


        public Action<T> BeforeSavingAction { get; set; }

        /// <summary>
        /// Save with Audit Trail motivation
        /// </summary>
        /// <param name="caption">Caption of Audit Trail window</param>
        /// <param name="iconPath">Path to icon image</param>
        /// <param name="sign">True to enforce signing</param>
        /// <param name="motivate">True to enforce motivation</param>
        /// <returns>True if saved</returns>
        public async Task<bool> SaveAsync(string caption, string iconPath, bool sign, bool motivate)
        {
            BeforeSavingAction?.Invoke(_entity);

            // used in unit tests : todo have test unit db
            if (_data == null) return true;

            var transaction = _data.GetTransaction();
            try
            {
                Message = null;

                var log = Persister.ToString();

                var action = _entityId < 0 ? "Create" : "Update";

                //TODO : add AclRight needed to do the action
                if (_getAudit(transaction).Audit(action, null, log, _entity, caption, iconPath, sign, motivate))
                {
                    await SaveAsync(transaction);
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

        public async Task SaveAsync(IDataTransaction transaction)
        {
            _ = await Persister.SaveAsync(transaction);
            _ = _timer.Change(Timeout.Infinite, Timeout.Infinite);

            if (_lock != null)
            {
                _ = await transaction.DeleteAsync(_lock);
            }

            IsActive = false;
            _lock = null;

            foreach (var locker in _dependencies)
            {
                await locker.SaveAsync(transaction);
            }
        }

        private readonly AsyncReaderWriterLock _cancelLock = new AsyncReaderWriterLock();

        // TODO : possible bug if dependency gets canceled
        public async Task DirtyCancelAsync()
        {
            using (_cancelLock.WriterLock())
            {
                _ = _timer.Change(Timeout.Infinite, Timeout.Infinite);

                try
                {
                    foreach (var locker in _dependencies)
                    {
                        await locker.DirtyCancelAsync();
                    }

                    _ = await _data.ReFetchOneAsync(_entity).ConfigureAwait(true);
                }
                catch { }

                _lock = null;
                Persister.Reset();
                IsActive = false;
            }
        }

        /// <summary>
        /// Abort editing
        /// </summary>
        /// <returns></returns>
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
                        if (_data.Delete(_lock))
                        {
                            _lock = null;
                            await _data.ReFetchOneAsync(_entity).ConfigureAwait(true);
                            Persister.Reset();
                            IsActive = false;
                        }
                    }

                    else IsActive = false;
                }

                await CancelDependenciesAsync();
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

        /// <summary>
        /// Message describing why lock is not possible
        /// </summary>
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
            if (IsActive)
                await CancelAsync().ConfigureAwait(false);
            await _timer.DisposeAsync();
        }

    }
}