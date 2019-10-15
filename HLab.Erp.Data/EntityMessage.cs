using System;

namespace HLab.Erp.Data
{
    public class EntityMessage
    {
        public bool IsSameOrAnotherOf(IEntity item)
        {
            throw new NotImplementedException();
        }
    }
    public class EntityMessage<T> : EntityMessage
    {
        public EntityMessage(int modelId)
        {
            throw new NotImplementedException();
        }
    }
}