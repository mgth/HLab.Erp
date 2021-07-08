using System;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.Wpf.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Acl.AuditTrails
{
    public class AuditTrailsListViewModel : EntityListViewModel<AuditTrail>, IMvvmContextProvider
    {
        public class AuditTrailsListBootloader : NestedBootloader
        {
            public override string MenuPath => "tools";
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        private static string LogAbstract(string log, int size)
        {
            const string suffix = "...";

            var result = log.Replace('\n', '/').Replace("\r", "");
            if (result.Length < size) return result;
            result = result.Substring(0, Math.Max(0, size - suffix.Length)) + suffix;
            return result;
        }

        public AuditTrailsListViewModel() : base(c => c
            .Column()
            .Header("{Date}").Width(110)
            .Link(at => at.TimeStamp)
            .Filter()
            .MaxDate(DateTime.Now)
            .MinDate(DateTime.Now - TimeSpan.FromDays(30))

            .Column()
            .Header("{Action}").Width(80)
            .Link(at => at.Action)
            .Filter()

            .Column()
            .Header("{Caption}").Width(200)
            .Link(at => at.EntityCaption)
            .Filter()

            .Column()
            .Header("{Class}").Width(80)
            .Link(at => at.EntityClass)
            .Filter()

            .Column()
            .Header("{Icon}").Width(60)
            .Icon(at => at.IconPath)

            .Column()
            .Header("{Log}").Width(350).Content(at => LogAbstract(at.Log, 50))

            .Column()
            .Header("{Motivation}").Width(250)
            .Link(at => at.Motivation)
            .Filter()

            .Column()
            .Header("{User}").Width(150)
            .Link(at => at.UserCaption)
            .Filter()
        )
        { }

    }
}