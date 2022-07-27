using System;
using HLab.Core.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.Tools.Details
{
    public class DetailsModule : IBootloader
    {
        readonly IDocumentService _docs;
        readonly Func<DetailsPanelViewModel> _getDetails;

        public DetailsModule(IDocumentService docs, Func<DetailsPanelViewModel> getDetails)
        {
            _docs = docs;
            _getDetails = getDetails;
        }

        public void Load(IBootContext b)
        {
            //    //TODO :
            //    //_docs.OpenDocument(_getDetails());
        }
    }
}