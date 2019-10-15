using System;

namespace HLab.Erp.Acl
{
    public interface IDataLocker : IDisposable
    {
        bool IsActive { get; set; }
    }
}
