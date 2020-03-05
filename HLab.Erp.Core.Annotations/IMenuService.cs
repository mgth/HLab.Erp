using System.Windows.Input;

namespace HLab.Erp.Core
{
    public interface IMenuService
    {
        void RegisterMenu(string path, object header, ICommand cmd, string getIcon);
    }
}
