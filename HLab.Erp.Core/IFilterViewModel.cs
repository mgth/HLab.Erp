using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using System;
using System.Linq.Expressions;

namespace HLab.Erp.Core
{
    public interface IFilterViewModel
    {
        bool Enabled { get; set; }
        object Header { get; set; }
        string IconPath { get; set; }

    }
}
