////using System.Data.Entity;

namespace HLab.Erp.Data
{


    //[Entity]
    public interface IEntity 
    {
        object Id{ get; }
        bool IsLoaded { get; set; }
        void OnLoaded();
    }
    public interface IEntity<T> : IEntity
    {
         new T Id{ get; set; }
    }
}
