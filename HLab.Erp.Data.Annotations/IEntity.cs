////using System.Data.Entity;

namespace HLab.Erp.Data
{


    //[Entity]
    public interface IEntity //: INotifierObject
    {
        object Id{ get; }
        bool IsLoaded { get; set; }
        void OnLoaded();

        //IErpContext Context { get; }
    }
    public interface IEntity<T> : IEntity
    {
         new T Id{ get; set; }
    }
}
