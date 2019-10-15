using System;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core
{
    public abstract class DocumentService : IDocumentService
    {
        [Import] private IMvvmService _mvvm { get; }
        [Import] private Func<Type, object> _getter { get; }

        public abstract void OpenDocument(IView content);

        public void OpenDocument(object obj)
        {
            if (obj is Type t)
            {
                obj = _getter(t);
            }

            if (obj is IView view)
                OpenDocument(view);
            else
            {
                var doc = _mvvm.MainContext.GetView(obj, typeof(ViewModeDefault), typeof(IViewClassDocument));
                OpenDocument(doc);
            }
        }
    }
}
