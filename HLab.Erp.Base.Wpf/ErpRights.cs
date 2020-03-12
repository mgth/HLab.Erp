using System;
using System.Collections.Generic;
using System.Text;
using HLab.Erp.Acl;

namespace HLab.Erp.Base.Wpf
{
    public static class ErpRights
    {
        /// <summary>
        /// Allow to create / Delete users
        /// </summary>        
        public static readonly AclRight SignCustomer = AclRight.Get();
    }
}
