using System;
using System.Linq;
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
        [Import] public IMessageBus MessageBus { get; private set; }

        [Import] private Func<object, SelectedMessage> GetMessage { get; set; }

        public override async Task OpenDocumentAsync(IView content)
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
        public override async Task CloseDocumentAsync(object content)
        {
            if (MainViewModel is MainWpfViewModel vm)
            {
                if (content is IView view)
                {
                    if (vm.Documents.Contains(view))
                    {
                        vm.Documents.Remove(view);
                        return;
                    }

                    if (vm.Anchorables.Contains(view))
                    {
                        vm.Anchorables.Remove(view);
                        return;
                    }
                }

                var documents = vm.Documents.OfType<FrameworkElement>().ToList();
                foreach (var document in documents)
                {
                    if (ReferenceEquals(document.DataContext, content))
                    {
                        vm.Documents.Remove(document);
                    }
                }

                var anchorables = vm.Anchorables.OfType<FrameworkElement>().ToList();
                foreach (var anchorable in anchorables)
                {
                    if (ReferenceEquals(anchorable.DataContext, content))
                    {
                        vm.Anchorables.Remove(anchorable);
                    }
                }

            }
        }
    }
}
