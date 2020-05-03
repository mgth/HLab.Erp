using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HLab.Erp.Core
{
    public interface IColumnsProvider<T>
    {
        void Populate(object grid);

        IColumnsProvider<T> Icon(string caption, Func<T, string> iconPath, Func<T, object> orderBy = null,
            double height = 25.0, string id = null);

        IColumnsProvider<T> Column(string caption, Func<T, object> f, string id = null);
        IColumnsProvider<T> Column(string caption, Func<T, object> getter, Func<T, object> orderBy, string id = null);

        IColumnsProvider<T> ColumnAsync(string caption, Func<T, Task<object>> f, Func<T, object> orderBy,
            string id = null);

        IColumnsProvider<T> Localize(string caption, Func<T, string> text, Func<T, object> orderBy = null,
            double height = 25.0, string id = null);

        IColumnsProvider<T> Hidden(string id, Func<T, object> f);

        object GetValue(T obj, string name);

        object GetView();
    }
}
