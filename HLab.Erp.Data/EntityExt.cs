using System;
using System.Runtime.CompilerServices;

namespace HLab.Erp.Data
{
    public static class EntityExt
    {
        //public static void Onloaded(this IEnumerable<IEntity> list)
        //{
        //    foreach (var e in list)
        //    {
        //        e?.OnLoaded();
        //    }
        //}


        //public static int DbGetId(this IEntity entity)
        //{
        //    //var proxy = entity.GetProxy();
        //    //if (!proxy.HasNotifier) return -1;//throw new Exception("ID has not been set");

        //    if(!(entity.GetNotifier() is EntityHelper notifier))  throw new Exception("Notifier should be EntityHelper");

        //    return notifier.Id;
        //}

        //public static bool DbSetId(this IEntity entity, int value)
        //{
        //    return entity.SetHelperId(value);
        //}

        public static T DbGet<TE, T>(this TE entity, [CallerMemberName] string propertyName = null)
            where TE : class, IEntity, new()
        {
            return entity.DbGet<TE,T>(a => default(T), propertyName);
        }

        public static T DbGet<TE, T>(this TE entity, Func<T> def, [CallerMemberName] string propertyName = null)
            where TE : class, IEntity
        {
            return entity.DbGet<TE,T>(a => def(), propertyName);
        }



        //public static EntityHelper<TE> GetEntityHelper<TE>(this TE entity)
        //    where TE : class, IEntity, new()
        //{
        //    var proxy = entity.GetProxy();
        //    if (proxy.Notifier == null) proxy.Notifier = EntityHelper<TE>.GetHelper(entity);

        //    if(!(proxy.Notifier is EntityHelper<TE>)) throw new InvalidCastException();

        //    return proxy.Notifier as EntityHelper<TE>;
        //}

        public static T DbGet<TE, T>(this TE entity, Func<T, T> def, [CallerMemberName] string propertyName = null)
            where TE : class, IEntity
        {
            //var proxy = entity.GetProxy();
            //if (proxy.Notifier == null) proxy.Notifier = EntityHelper<TE>.GetHelper(entity);

            return default(T); //TODO : NotifierObjectExt.GetNotifier(entity).Get( def, propertyName);
        }

        public static string AnchorId<T>(this T entity)
            where T : IEntity
        {
            if (entity == null) return null;
            return entity.GetType().Name + "_" + entity.Id;
        }



    }
}