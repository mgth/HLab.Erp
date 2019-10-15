using System;

namespace HLab.Erp.Core
{
    public class IsListColumn : Attribute
    {
        public IsListColumn(string header = null)
        {
            Header = header;
        }
        public string Header { get; }
    }

}
