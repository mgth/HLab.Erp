using System;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Acl.AuditTrails
{
    public class AuditTrailsDataModule : ErpParamModule<AuditTrailsListViewModel>
    {
    }

    public class AuditTrailsListViewModel : EntityListViewModel<AuditTrail>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        private string LogAbstract(string log, int size)
        {
            const string suffix = "...";

            var result = log.Replace('\n', '/').Replace("\r","");
            if (result.Length < size) return result;
            result = result.Substring(0, Math.Max(0,size - suffix.Length)) + suffix;
            return result;
        }

        public AuditTrailsListViewModel()
        {
            DeleteAllowed = false;
            AddAllowed = false;

            Columns.Configure(c => c
                .Column
                    .Header("{Date}").Width(110)
                    .Content(at => at.TimeStamp)
                .Column
                    .Header("{Action}").Width(80)
                    .Content(at => at.Action)
                .Column
                    .Header("{Caption}").Width(200)
                    .Content(at=>at.EntityCaption)
                .Column
                    .Header("{Class}").Width(80)
                    .Content(at=>at.EntityClass)
                .Column
                    .Header("{Icon}").Width(60)
                    .Icon( at=>at.IconPath)
                .Column
                    .Header("{Log}").Width(350)
                    .Content(at=>LogAbstract(at.Log,50))
                .Column
                    .Header("{Motivation}").Width(250)
                    .Content(at=>at.Motivation)
                .Column
                    .Header("{User}").Width(150)
                    .Content(at=>at.UserCaption)
            );
                ;

            AddFilter<DateFilter>(f =>  f
                .Title("{Date}")
                .MaxDate(DateTime.Now)
                .MinDate(DateTime.Now - TimeSpan.FromDays(30))
                .Link(this,a => a.TimeStamp)
            );

            AddFilter<TextFilter>(f =>  f
                .Title("{Action}")
                .Link(this,a => a.Action)
            );

            AddFilter<TextFilter>(f => f
                .Title("{Caption}")
                .Link(this,at=>at.EntityCaption)
            );

            AddFilter<TextFilter>(f => f
                .Title("{Class}")
                .Link(this,at=>at.EntityClass)
            );

            List.Update();
        }
    }
    public class AuditTrailViewModel : EntityViewModel<AuditTrail>
    {

    }
}