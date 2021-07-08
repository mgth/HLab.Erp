using System;
using HLab.Erp.Data;

namespace HLab.Erp.Base.Data
{
    public interface ISqlBuilder
    {
        ISqlBuilder SqlResource(string filePath);
        ISqlBuilder Version(string filePath);

        ISqlBuilder Include(Func<string, ISqlBuilder, ISqlBuilder> callFunc);
        ISqlTableBuilder<T> Table<T>() where T : class, IEntity;

    }
}