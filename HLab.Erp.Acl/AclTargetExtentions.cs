using System.Collections.Generic;
using System.Linq;
using HLab.Erp.Data;

namespace HLab.Erp.Acl
{
    public static class AclTargetExtentions
    {
        public static bool HasRight(this User user, AclRight right) => true; // TODO


        public static AclList GetAclList(this IAclTarget target, IDataService dbService)
        {
            return dbService.FetchOne<AclList>(al => al.Target == target.AclTargetId);
        }
        public static AclList GetOrAddAclList(this IAclTarget target, IDataService dbService)
        {
                return dbService.GetOrAdd<AclList>(al => al.Target == target.AclTargetId, al =>
                {
                    al.ParentId = target.Parent?.AclTargetId;
                 });            
        }

        public static bool IsGranted(this IAclTarget target, string right, AclNode group, IDataService dbService)
        {
            var groups = group.GetGroups().ToList();
            return target.IsGranted(right, groups,dbService);
        }

        public static bool IsGranted(this IAclTarget target, string right, IList<int> groups, IDataService dbService)
        {
            bool granted = false;
            while (true)
            {
                var list = target.GetAclList(dbService);
                foreach (var g in list.Granted)
                {
                    if (g.Right == right && groups.Contains(g.AclNodeId))
                    {
                        if (g.Deny) return false;
                        granted = true;
                    }
                }
                if(!list.Inherit || target.Parent==null) return granted;
                target = target.Parent;
            }
        }

    }
}
