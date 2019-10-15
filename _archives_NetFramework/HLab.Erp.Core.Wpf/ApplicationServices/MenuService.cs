using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Wpf.Lang;


namespace HLab.Erp.Core.Wpf.ApplicationServices
{
    [Export(typeof(IMenuService)), Singleton]
    public class MenuService : IMenuService
    {
        [Import]
        private MainWpfViewModel _viewModel;

        public void RegisterMenu(string parent, string newName, object header, ICommand command, object icon)
        {
            if (header is string s)
            {
                header = new Localize {Id = s};
            }

            var m = new MenuItem
            {
                Name = newName,
                Header = header,
                Command = command,
                Icon = new Viewbox
                {
                    Height = 25,
                    Child = icon as UIElement
                }

            };
            _viewModel.RegisterMenu(parent?.Split('/') ?? new string[] { }, m);
        }
    }
}
