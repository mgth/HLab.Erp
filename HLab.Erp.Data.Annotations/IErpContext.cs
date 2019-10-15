using System;
using System.Collections.Generic;
using System.Text;

namespace HLab.Erp.Data
{
    public interface IErpContext
    {
        IDataService Db { get; }
    }
}
