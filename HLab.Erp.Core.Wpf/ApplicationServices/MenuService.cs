using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Icons;
using HLab.Mvvm.Lang;

namespace HLab.Erp.Core.ApplicationServices
{
    [Export(typeof(IMenuService)), Singleton]
    public class MenuService : IMenuService
    {
        [Import]
        private MainWpfViewModel _viewModel;

        public void RegisterMenu(string parent, string newName, object header, ICommand command, string iconPath)
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
                Icon = new IconView
                {
                    Height = 25,
                    Path = iconPath
                }

            };
            _viewModel.RegisterMenu(parent?.Split('/') ?? new string[] { }, m);
        }
    }
}
