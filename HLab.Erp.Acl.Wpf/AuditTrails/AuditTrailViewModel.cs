using System;
using System.Linq;
using HLab.Erp.Acl.Users;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;
using Microsoft.AspNetCore.Http.Features;

namespace HLab.Erp.Acl.AuditTrails
{
    public class AuditTrailsDataModule : ErpParamModule<AuditTrailsDataModule, AuditTrailsListViewModel>
    {
    }

    public class AuditTrailsListViewModel : EntityListViewModel<AuditTrailsListViewModel,AuditTrail>, IMvvmContextProvider
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

            Columns
                .Column("{Date}", at => at.TimeStamp)
                .Column("{Action}", at => at.Action)
                .Column("{Caption}", at=>at.EntityCaption)
                .Column("{Class}", at=>at.EntityClass)
                .Icon("{Icon}", at=>at.IconPath)
                .Column("{Log}", at=>LogAbstract(at.Log,50))
                .Column("{Motivation}", at=>at.Motivation)
                .Column("{User}", at=>at.UserCaption)
                ;

            AddFilter<FilterDateViewModel>(f =>  f
                .Title("{Date}")
                .MaxDate(DateTime.Now)
                .MinDate(DateTime.Now - TimeSpan.FromDays(30))
                .Link(this,a => a.TimeStamp)
            );

            AddFilter<FilterTextViewModel>(f =>  f
                .Title("{Action}")
                .Link(this,a => a.Action)
            );

            AddFilter<FilterTextViewModel>(f => f
                .Title("{Caption}")
                .Link(this,at=>at.EntityCaption)
            );

            AddFilter<FilterTextViewModel>(f => f
                .Title("{Class}")
                .Link(this,at=>at.EntityClass)
            );

            List.UpdateAsync();
        }
    }
    public class AuditTrailViewModel : EntityViewModel<AuditTrailViewModel, AuditTrail>
    {

    }
}