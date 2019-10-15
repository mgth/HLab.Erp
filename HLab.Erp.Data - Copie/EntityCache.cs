using System;
using System.Collections.Generic;
////using System.Data.Entity;

namespace HLab.Erp.Data
{
    class EntityCache
    {
        private readonly object _lockCache = new object();
        private readonly Dictionary<int, WeakReference<EntityHelper>> _cache 
            = new Dictionary<int, WeakReference<EntityHelper>>();


        private readonly Type _type;
        public EntityCache(Type type)
        {
            this._type = type;
        }
        //public EntityHelper GetOrAddHelper(int id)
        //{
        //    lock (_lockCache)
        //    {
        //        var helper = GetExistingHelper(id);
        //        if (helper != null) return helper;

        //        helper = new EntityHelper(_type, id);

        //        if(id>=0)
        //            _cache.Add(id, new WeakReference<EntityHelper>(helper));

        //        return helper;
        //    }
        //}
        //public EntityHelper GetExistingHelper(int id)
        //{
        //    lock (_lockCache)
        //    {
        //        if (id >= 0 && _cache.ContainsKey(id))
        //        {
        //            var w = _cache[id];
        //            if (w.TryGetTarget(out EntityHelper target)) return target;
        //            _cache.Remove(id);
        //        }

        //        return null;
        //    }
        //}

        public void DropCache(int? id)
        {
            if (id == null) return;
            lock (_lockCache)
            {
                if (_cache.ContainsKey(id.Value))
                {
                    _cache.Remove(id.Value);
                }
                else throw new Exception("Entity not found in cache");                
            }
        }
    }
}
