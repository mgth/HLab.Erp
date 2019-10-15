using System;
using System.Windows;
using HLab.Core;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.ApplicationServices
{
    [Export(typeof(IDocumentService))]
    public class DocumentServiceWpf : DocumentService
    {
        [Import]
        public IMessageBus MessageBus { get; private set; }

        [Import]
        public Func<MainWpfViewModel> GetViewModel { get; set; }
        [Import] private Func<object, SelectedMessage> GetMessage { get; set; }
        public override void OpenDocument(IView content)
        {
            var vm = GetViewModel();

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
