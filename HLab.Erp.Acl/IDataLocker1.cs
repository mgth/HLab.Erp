using HLab.Erp.Data;


namespace HLab.Erp.Acl
{
    public interface IDataLocker<T> : IDataLocker
    where T : class,IEntity<int>
    {
            EntityPersister<T> Persister { get; }
    }
}