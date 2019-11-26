using System;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core
{
    public abstract class DocumentService : IDocumentService
    {
        [Import] private IMvvmService _mvvm { get; }
        [Import] private Func<Type, object> _getter { get; }

        public abstract Task OpenDocument(IView content);

        public object MainViewModel {get;set;}

        public async Task OpenDocument(object obj)
        {
            if (obj is Type t)
            {
                obj = _getter(t);
            }

            if (obj is IView view)
                await OpenDocument(view);
            else
            {
                var doc = _mvvm.MainContext.GetView(obj, typeof(ViewModeDefault), typeof(IViewClassDocument));
                await OpenDocument(doc);
            }
        }
    }
}
