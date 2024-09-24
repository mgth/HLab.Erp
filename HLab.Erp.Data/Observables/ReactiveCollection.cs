/*
  HLab.Mvvm
  Copyright (c) 2021 Mathieu GRENET.  All right reserved.

  This file is part of HLab.Mvvm.

    HLab.Mvvm is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    HLab.Mvvm is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MouseControl.  If not, see <http://www.gnu.org/licenses/>.

	  mailto:mathieu@mgth.fr
	  http://www.mgth.fr
*/

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using DynamicData;
using DynamicData.Binding;
using HLab.Base;
using Nito.AsyncEx;
using ReactiveUI;

namespace HLab.Erp.Data.Observables
{
    public abstract class ReactiveCollection2<T> : IObservableList<T>
    {
        public int Count => throw new NotImplementedException();

        public IObservable<int> CountChanged => throw new NotImplementedException();

        public IReadOnlyList<T> Items => throw new NotImplementedException();

        public IObservable<IChangeSet<T>> Connect(Func<T, bool>? predicate = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IObservable<IChangeSet<T>> Preview(Func<T, bool>? predicate = null)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class ReactiveCollection<T> : ReactiveObject, IObservableList<T>,
        IList<T>, IList, IReadOnlyList<T>, INotifyCollectionChanged, ILockable
    {
        protected ReactiveCollection()
        {
            this.WhenAnyValue(e => e.Selected).Subscribe(s => Debug.WriteLine("Selected " + s));                                        
        }


        #region Dependencies

        #endregion

        readonly SourceList<T> _list = new();

        protected AsyncReaderWriterLock Lock { get; } = new();

        readonly ConcurrentQueue<NotifyCollectionChangedEventArgs> _changedQueue = new();

        public event NotifyCollectionChangedEventHandler CollectionChanged;


        public virtual T Selected
        {
            get => DoReadLocked(() => _selected);
            set => DoWriteLocked(() => this.RaiseAndSetIfChanged(ref _selected, value));
        }
        T _selected;

        public IEnumerator<T> GetEnumerator() => _list.Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.Items.GetEnumerator();

        public IObservable<IChangeSet<T>> Connect(Func<T, bool>? predicate = null)
        {
            return _list.Connect(predicate);
        }

        public IObservable<IChangeSet<T>> Preview(Func<T, bool>? predicate = null)
        {
            return _list.Preview(predicate);
        }

        public int Count
        {
            get => _count;
            private set => this.RaiseAndSetIfChanged(ref _count, value);
        }

        public IObservable<int> CountChanged => _list.CountChanged;

        public IReadOnlyList<T> Items => _list.Items;

        int _count;

        int IList.Add(object item) => _add((T)item);
        public virtual void Add(T item) => _add(item);

        int _add(T item)
        {
            try
            {
                using (Lock.WriterLock())
                { 
                    var index = _list.Count;
                    _list.Add(item);
                    Count = _list.Count;
                    _changedQueue.Enqueue(new(NotifyCollectionChangedAction.Add, item, index));
                    return index;
                }
            }
            finally
            {
                OnCollectionChanged();
            }
        }

        public bool AddUnique(T item) => DoWriteLocked(() =>
        {
            if (Contains(item)) return false;
            var index = Count;
            _list.Add(item);
            Count = _list.Count;
            _changedQueue.Enqueue(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            return true;
        });


        public bool Remove(T item) => DoWriteLocked(() =>
            {
                var index = _list.Items.IndexOf(item);
                var r = _list.Remove(item);
                Count = _list.Count;
                _changedQueue.Enqueue(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                return r;
            }
        );




        protected void RemoveAtNoLock(int index)
        {
            var item = ((IList)_list.Items)[index];
            _list.RemoveAt(index);
            Count = _list.Count;
            _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        public void RemoveAt(int index) => DoWriteLocked(() => RemoveAtNoLock(index));



        public virtual void Insert(int index, T item) => DoWriteLocked(() => InsertNoLock(index, item));

        protected void InsertNoLock(int index, T item)
        {
            Debug.Assert(index <= _list.Count);
            _list.Insert(index, item);
            Count = _list.Count;
            _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }



        public void CopyTo(Array array, int index) => DoReadLocked(() =>
        {
            ((IList)_list.Items).CopyTo(array, index);
        });

        public void CopyTo(T[] array, int arrayIndex) => DoReadLocked(() =>
        {
            ((IList)_list.Items).CopyTo(array, arrayIndex);
        });

        public void Clear() => DoWriteLocked(() =>
        {
            _list.Clear();
            _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        });

        bool IList.Contains(object value) => DoReadLocked(() => ((IList)_list.Items).Contains(value));

        public bool Contains(T item) => DoReadLocked(() => _list.Items.Contains(item));

        void IList.Insert(int index, object value) => Insert(index, (T)value);
        void IList.Remove(object item) => Remove((T)item);
        int IList.IndexOf(object value) => DoReadLocked(() => ((IList)_list.Items).IndexOf(value));
        public int IndexOf(T item) => DoReadLocked(() => _list.Items.IndexOf(item));
        object ICollection.SyncRoot => DoReadLocked(() => ((ICollection)_list.Items).SyncRoot);
        bool ICollection.IsSynchronized => DoReadLocked(() => ((ICollection)_list.Items).IsSynchronized);
        bool IList.IsFixedSize => DoReadLocked(() => ((IList)_list.Items).IsFixedSize);
        bool ICollection<T>.IsReadOnly => DoReadLocked(() => ((ICollection<T>)_list.Items).IsReadOnly);
        bool IList.IsReadOnly => DoReadLocked(() => ((IList)_list.Items).IsReadOnly);

        object IList.this[int index]
        {
            get => DoReadLocked(() => ((IList)_list.Items)[index]);
            set => DoWriteLocked(() =>
                {
                    ((IList)_list.Items)[index] = value;
                    Count = _list.Count;
                    _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, index));
                }
            );
        }

        protected T GetNoLock(int index) => (T)((IList)_list.Items)[index];

        public T this[int index]
        {
            get => DoReadLocked(() => (T)((IList)_list.Items)[index]);
            set => DoWriteLocked(() =>
            {
                ((IList)_list.Items)[index] = value;
                Count = _list.Count;
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, index));
            });
        }

        protected void OnCollectionChanged()
        {
            while (_changedQueue.TryDequeue(out var a))
                OnCollectionChanged(a);
        }

        void OnCollectionChanged(NotifyCollectionChangedEventArgs arg)
        {
            CollectionChanged?.Invoke(this, arg);
        }

        AsyncReaderWriterLock ILockable.Lock => Lock;

        TR DoReadLocked<TR>(Func<TR> action)
        {
            using (Lock.ReaderLock())
            {
                return action();
            }
        }

        void DoReadLocked(Action action)
        {
            using (Lock.ReaderLock())
            {
                action();
            }
        }

        TR DoWriteLocked<TR>(Func<TR> action)
        {
            try
            {
                using (Lock.WriterLock())
                {
                    using (DelayChangeNotifications())
                        return action();
                }
            }
            finally
            {
                OnCollectionChanged();
            }
        }

        void DoWriteLocked(Action action)
        {
            try
            {
                using (Lock.WriterLock())
                {
                    using (DelayChangeNotifications())
                        action();
                }
            }
            finally
            {
                OnCollectionChanged();
            }
        }

        public void Dispose()
        {
            _list.Dispose();
        }
    }
}
