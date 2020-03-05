using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Icons;
using HLab.Mvvm.Lang;

namespace HLab.Erp.Core.ApplicationServices
{
    class MenuPath
    {
        public string Name {get;} = "";
        public MenuPath Next {get;} = null;
        public MenuPath(string path):this(path.Split('/')) {  }
        public MenuPath(IEnumerable<string> path)
        {
            Name = path.First();
            if(path.Count()>1)
                Next = new MenuPath(path.Skip(1));
        }
    }


    [Export(typeof(IMenuService)), Singleton]
    public class MenuService : IMenuService
    {
        [Import]
        private MainWpfViewModel _viewModel;

        public void RegisterMenu(string path, object header, ICommand command, string iconPath) 
            => RegisterMenu(new MenuPath(path),_viewModel.Menu.Items, header, command, iconPath);

        private void RegisterMenu(MenuPath path, ItemCollection items, object header, ICommand command, string iconPath)
        {
            if (path.Next==null)
            {
                if (header is string s)
                {
                    header = new Localize {Id = s};
                }

                var m = new MenuItem
                {
                    Name = path.Name,
                    Header = header,
                    Command = command,
                    Icon = new IconView
                    {
                        Height = 25,
                        Path = iconPath
                    }
                };

                foreach(MenuItem menu in items)
                {
                    if (menu.Name == path.Name)
                    {
                        items.Remove(menu);
                        foreach(var sub in menu.Items)
                        {
                            m.Items.Add(sub);
                        }
                    }
                }

                items.Add(m);
                return;

            }

            MenuItem child = null;
            foreach (MenuItem menu in items)
            {
                if (menu.Name == path.Name)
                {
                    child = menu;
                    break;
                }
            }

            if(child==null) 
            {
                child = new MenuItem{Name = path.Name};
                items.Add(child);
            }
            RegisterMenu(path.Next, child.Items, header,command,iconPath);
        }

    }
}
