using System;
using HLab.Erp.Data;

namespace HLab.Erp.Core.Wpf
{
    public class EntityString : Entity<EntityString,int>, IEntity
    {

        private static int _cnt = 0;
        private int _id = -1;
        public override bool IsLoaded
        {
            get => true;
            set => throw new NotImplementedException();
        }

        public EntityString()
        {
            String = "";
        }

        public EntityString(string s)
        {
            String = s;
        }

        public override int Id
        {
            get
            {
                if (_id == -1) return _id = _cnt++;
                else return _id;
            }
            set => _id = value;
        }

        public string String { get; set; }

        public override string ToString()
        {
            return String;
        }
        public static implicit operator string(EntityString s)
        {
            return s?.String;
        }
        public static implicit operator EntityString(string s)
        {
            return new EntityString(s);
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

    }
}
