using System;
using HLab.Erp.Data;


namespace HLab.Erp.Acl
{
    public class DataLockerEntityDesign : IEntity<int>
    {
        int _id;
        public object Id { get; } = 1;

        int IEntity<int>.Id
        {
            get => 1;
            set => throw new InvalidOperationException();
        }

        public bool IsLoaded { get; set; } = true;
        public void OnLoaded()
        {
            throw new InvalidOperationException();
        }
    }
}