using System;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using HLab.Base.ReactiveUI;
using HLab.Erp.Data;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;


namespace HLab.Erp.Acl.Users;

public class AdUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Login { get; set; }
    public required string Function { get; set; }
    public required string Email { get; set; }

    public override string ToString() => $"{Login} : {FirstName} {LastName}";
}

public class ImportUsersViewModel : ViewModel
{
    public IAclService Acl;
    readonly IDataService _data;
    public ImportUsersViewModel(IAclService acl, IDataService data)
    {
        Acl = acl;
        _data = data;
            

        Domain = Acl.Connection.User.Domain;
        UserName = Acl.Connection.User.Username;

        RetrieveUsersCommand = ReactiveCommand.Create<PasswordBox>((a) => RetrieveUsers(a as PasswordBox));

        ImportUserCommand = ReactiveCommand.Create(
            ImportUsers, 
            this
                .WhenAnyValue(e => e.Acl.Connection.User)
                .Select(u => acl.IsGranted(AclRights.ManageUser)));
    }

    public string Title => "{Import Users}";


    public ObservableCollection<AdUser> Users { get; } = [];

    public string Domain { get => _domain; set => this.SetAndRaise(ref _domain,value); }
    string _domain ;

    public string Message { get => _message; set => this.SetAndRaise(ref _message,value); }
    string _message ;

    public bool Success { get => _success; set => this.SetAndRaise(ref _success,value); }
    bool _success ;

    public string UserName { get => _userName; set => this.SetAndRaise(ref _userName,value); }
    string _userName ;
    public AdUser? SelectedUser { get => _selectedUser; set => this.SetAndRaise(ref _selectedUser,value); }
    AdUser? _selectedUser ;

    public ICommand RetrieveUsersCommand { get; }

    void RetrieveUsers(PasswordBox password)
    {
        try
        {
            using var context = new PrincipalContext(ContextType.Domain, Domain, UserName, string.IsNullOrEmpty(password.Password)?null:password.Password);
            using var searcher = new PrincipalSearcher(new UserPrincipal(context));

            Users.Clear();

            foreach (var result in searcher.FindAll())
            {
                if(result.GetUnderlyingObject() is not DirectoryEntry de) continue;
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
            var DomainPath = "LDAP://" + string.Join(",", dcs.Select(e => "DC=" + e));
            var searchRoot = new DirectoryEntry(DomainPath);
            var search = new DirectorySearcher(searchRoot);
            search.Filter = "(&(objectClass=user)(objectCategory=person))";
            search.PropertiesToLoad.Add("samaccountname");
            search.PropertiesToLoad.Add("mail");
            search.PropertiesToLoad.Add("usergroup");
            search.PropertiesToLoad.Add("displayname");//first name
            var resultCol = search.FindAll();
            if (resultCol == null) return;

            for (var counter = 0; counter < resultCol.Count; counter++)
            {
                var result = resultCol[counter];

                if (!result.Properties.Contains("samaccountname") ||
                    !result.Properties.Contains("mail") ||
                    !result.Properties.Contains("displayname")) continue;

                var objSurveyUsers = new AdUser
                {
                    Email = (string)result.Properties["mail"][0] +
                            "^" +
                            (string)result.Properties["displayname"][0],
                    Login = (string)result.Properties["samaccountname"][0],
                    FirstName = (string)result.Properties["givenName"][0],
                    LastName = (string)result.Properties["sn"][0],
                    Function = null
                };
                Users.Add(objSurveyUsers);
            }
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            Success = false;
        }
    }

    public ICommand ImportUserCommand { get; } 

    void ImportUsers()
    {
        var user = SelectedUser;
        if (user == null) return;

        var existingUser = _data.FetchOne<User>(u => u.Username == user.Login);
        if (existingUser != null)
        {
            existingUser.Domain = Domain;
            existingUser.Name = user.LastName;
            existingUser.FirstName = user.FirstName;
            existingUser.Email = user.Email;
            existingUser.Function = user.Function;
            existingUser.Initials = string.Concat(user.FirstName[..1].ToUpper(), user.LastName[..1].ToUpper());

            _data.Update(existingUser, "Domain", "Name", "Email", "Function", "Initials");

            Message = $"{{User}} {existingUser.Username} {{updated}}";
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
            u.Username = user.Login;
            u.Initials = user.FirstName[..1].ToUpper() + user.LastName[..1].ToUpper();
        }, u =>
        {
            Message = $"{{User}} {u.Username} {{created}}";
            Success = true;
            SelectedUser = null;
        });
    }
}