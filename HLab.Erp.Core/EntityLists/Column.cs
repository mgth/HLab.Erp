using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.EntityLists
{
    public class Column<T> : NotifierBase, IColumn<T> where T : class
    {
        internal Column()
        {
            Id = "C" + Guid.NewGuid().ToString().Replace('-', '_');
            H<Column<T>>.Initialize(this);
        }

        public bool Hidden { get; set; } = false;

        public object Header { get; set; } = "";
        public string IconPath { get; set; } = "";

        public double Width { get; set; } = double.NaN;

        public Func<T, object> OrderBy { get;  set; }

        public SortDirection SortDirection { get;  set; }

        public Func<T, object> Getter { get; set; }

        public IColumn<T> OrderByNext { get; set; }

//        ConditionalWeakTable<T,object> _cache = new();
        private Stopwatch _watch = new Stopwatch();
        private long _requestCount = 0;

        public long Benchmark => _benchmark.Get();
        private IProperty<long> _benchmark = H<Column<T>>.Property<long>();

        public object GetValue(T value)
        {
            #if DEBUG
            if (Getter is null) return "<Null>";
            #endif

            try
            {
//                return _cache.GetValue(value, v => Getter(v));
                _requestCount++;
                _watch.Start();
                return Getter(value);
            }
            catch(NullReferenceException)
            {
                return null;
            }
            finally
            {
                _watch.Stop();
                _benchmark.Set(_watch.ElapsedTicks / _requestCount);
            }
        }

        public string Id { get; set; }

        public override string ToString()
        {
            return Header.ToString();
        }
    }
}