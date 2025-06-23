using System;
using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Core.Tools.Details;

public class DetailsModule(IDocumentService docs, Func<DetailsPanelViewModel> getDetails)
    : Bootloader
{
    readonly IDocumentService _docs = docs;
    readonly Func<DetailsPanelViewModel> _getDetails = getDetails;

    protected override BootState Load()
    {
        //    //TODO :
        //    //_docs.OpenDocument(_getDetails());
        return base.Load();
   }

}