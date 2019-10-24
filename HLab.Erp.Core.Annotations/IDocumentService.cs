using System;
using System.Threading.Tasks;

namespace HLab.Erp.Core
{
    public interface IDocumentService
    {
         Task OpenDocument(object content);
    }
}
