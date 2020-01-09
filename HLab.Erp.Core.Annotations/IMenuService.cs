using System.Windows.Input;

namespace HLab.Erp.Core
{
    public interface IMenuService
    {
        void RegisterMenu(string parent, string newName, object header, ICommand cmd, string getIcon);
    }
}
