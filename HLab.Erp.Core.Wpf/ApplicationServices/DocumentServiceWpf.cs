using System;
using System.Threading.Tasks;
using System.Windows;
using HLab.Core;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.ApplicationServices
{
    [Export(typeof(IDocumentService)),Singleton]
    public class DocumentServiceWpf : DocumentService
    {
        [Import]
        public IMessageBus MessageBus { get; private set; }


        [Import] private Func<object, SelectedMessage> GetMessage { get; set; }
        public override async Task OpenDocument(IView content)
        {
            if (MainViewModel is MainWpfViewModel vm)
            {
                if (content is IViewClassAnchorable)
                {
                    if (!vm.Anchorables.Contains(content))
                        vm.Anchorables.Add(content);

                }
                else
                {
                    if (!vm.Documents.Contains(content))
                    {
                        vm.Documents.Add(content);

                        var message = GetMessage(content);

                        MessageBus.Publish(message);
                    }

                }

                vm.ActiveDocument = content as FrameworkElement;
            }
        }
    }
}
