using System;
using System.Threading.Tasks;

namespace HLab.Erp.Core
{
    public interface IDocumentService
    {
        Task OpenDocumentAsync(object content);

        object MainViewModel { get; set; }
    }
}
