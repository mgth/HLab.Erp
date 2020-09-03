using System;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;

namespace HLab.Erp.Core.Tools.Details
{
    public class DetailsModule : IBootloader //postboot
    {
        [Import] private readonly IDocumentService _docs;
        [Import] private readonly Func<DetailsViewModel> _getDetails;

        public void Load(IBootContext b)
        {
            //    //TODO :
            //    //_docs.OpenDocument(_getDetails());
        }
    }
}