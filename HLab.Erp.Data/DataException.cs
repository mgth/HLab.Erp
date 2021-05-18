using System;

namespace HLab.Erp.Data
{
    public class DataException : Exception
    {
        public DataException(string message, Exception inner = null) : base(message, inner)
        {

        }
    }
}