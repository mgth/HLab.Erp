using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl
{
    public class AdUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Function { get; set; }
        public string Email { get; set; }

        public override string ToString() => Login + " : " + FirstName + " " + LastName;
    }

    public class ImportUsersViewModel : ViewModel<ImportUsersViewModel>
    {
        public string Title => "{Import Users}";

        [Import] private IAclService _acl;
        [Import] private IDataService _data;

        public ObservableCollection<AdUser> Users { get; } = new ObservableCollection<AdUser>();

        public string Domain { get => _domain.Get(); set => _domain.Set(value); }
        private IProperty<string> _domain = H.Property<string>();

        public string Message { get => _message.Get(); set => _message.Set(value); }
        private IProperty<string> _message = H.Property<string>();

        public bool Success { get => _success.Get(); set => _success.Set(value); }
        private IProperty<bool> _success = H.Property<bool>();

        public string UserName { get => _userName.Get(); set => _userName.Set(value); }
        private IProperty<string> _userName = H.Property<string>();
        public AdUser SelectedUser { get => _selectedUser.Get(); set => _selectedUser.Set(value); }
        private IProperty<AdUser> _selectedUser = H.Property<AdUser>();

        public ICommand RetrievetUsersCommand { get; } = H.Command(c => c
              .Action((e, a) => e.RetrieveUsers(a as PasswordBox)));

        private void RetrieveUsers(PasswordBox password)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, Domain, UserName, String.IsNullOrEmpty(password.Password)?null:password.Password);
                using var searcher = new PrincipalSearcher(new UserPrincipal(context));

                Users.Clear();

                foreach (var result in searcher.FindAll())
                {
                    DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                    var user = new AdUser
                    {
                        FirstName = de.Properties["givenName"].Value?.ToString() ?? "",
                        LastName = de.Properties["sn"].Value?.ToString() ?? "",
                        Login = (de.Properties["userPrincipalName"].Value?.ToString() ?? "").Replace("@" + Domain, ""),
                        Email = de.Properties["email"].Value?.ToString() ?? "",
                        Function = de.Properties["description"].Value?.ToString() ?? ""
                    };

                    Users.Add(user);
                }
            }
            catch (PrincipalServerDownException ex)
            {
                Message = ex.Message;
                Success = false;
            }
            catch (DirectoryServicesCOMException ex)
            {
                Message = ex.Message;
                Success = false;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                Success = false;
            }
        }

        public void GetADUsers()
        {
            try
            {
                var dcs = Domain.Split('.').Reverse();
                string DomainPath = "LDAP://" + String.Join(",", dcs.Select(e => "DC=" + e));
                DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
                DirectorySearcher search = new DirectorySearcher(searchRoot);
                search.Filter = "(&(objectClass=user)(objectCategory=person))";
                search.PropertiesToLoad.Add("samaccountname");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("usergroup");
                search.PropertiesToLoad.Add("displayname");//first name
                SearchResult result;
                SearchResultCollection resultCol = search.FindAll();
                if (resultCol != null)
                {
                    for (int counter = 0; counter < resultCol.Count; counter++)
                    {
                        string UserNameEmailString = string.Empty;
                        result = resultCol[counter];
                        if (result.Properties.Contains("samaccountname") &&
                                 result.Properties.Contains("mail") &&
                            result.Properties.Contains("displayname"))
                        {
                            AdUser objSurveyUsers = new AdUser();
                            objSurveyUsers.Email = (String)result.Properties["mail"][0] +
                              "^" + (String)result.Properties["displayname"][0];
                            objSurveyUsers.Login = (String)result.Properties["samaccountname"][0];
                            objSurveyUsers.FirstName = (String)result.Properties["givenName"][0];
                            objSurveyUsers.LastName = (String)result.Properties["sn"][0];
                            Users.Add(objSurveyUsers);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                Success = false;
            }
        }

        public ICommand ImportUserCommand { get; } = H.Command(c => c
              .CanExecute(e => e._acl.IsGranted(AclRights.ManageUser))
              .Action(e => e.ImportUsers()));
        private void ImportUsers()
        {
            var user = SelectedUser;
            if (user == null) return;

            var existingUser = _data.FetchOne<User>(u => u.Login == user.Login);
            if (existingUser != null)
            {
                existingUser.Domain = Domain;
                existingUser.Name = user.LastName;
                existingUser.FirstName = user.FirstName;
                existingUser.Email = user.Email;
                existingUser.Function = user.Function;
                existingUser.Initials = user.FirstName.Substring(0, 1).ToUpper() + user.LastName.Substring(0, 1);

                _data.Update(existingUser, new[] { "Domain", "Name", "Email", "Function", "Initials" });

                Message = String.Format("{{User}} {0} {{updated}}",existingUser.Login);
                Success = true;

                return;
            }

            _data.Add<User>(u =>
            {
                u.Domain = Domain;
                u.FirstName = user.FirstName;
                u.Name = user.LastName;
                u.Email = user.Email;
                u.Function = user.Function;
                u.Login = user.Login;
                u.Initials = user.FirstName.Substring(0, 1).ToUpper() + user.LastName.Substring(0, 1);
            }, u =>
            {
                Message = String.Format("{{User}} {0} {{created}}",u.Login);
                Success = true;
                SelectedUser = null;
            });
        }
    }
}
