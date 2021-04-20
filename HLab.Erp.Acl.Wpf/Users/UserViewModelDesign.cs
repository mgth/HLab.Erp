using HLab.Mvvm.Annotations;

namespace HLab.Erp.Acl.Users
{
    public class UserViewModelDesign : UserViewModel, IViewModelDesign
    {
        public UserViewModelDesign() : base(null,null,null)
        {
            Model = new User
            {
                FirstName = "Gaston",
                Name = "Ouedraogo",
                Login = "g.ouedra",
                Function = "Technicien"
            };
        }
    }
}