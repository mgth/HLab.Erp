﻿using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Acl.Users;

public class ProfilesPerUserListViewModel : Core.EntityLists.EntityListViewModel<UserProfile>
{
    public ProfilesPerUserListViewModel(IDocumentService docs, Injector i, User user) : base(i, c => c
        .StaticFilter(e => e.UserId == user.Id)
        .Column("Name")
        .Header("{Name}")
        .Content(s => s.Profile.Name)
    )
    {
        OpenAction = target => docs.OpenDocumentAsync(target.Profile);
    }

    //public UserProfileListViewModel(Profile profile) : base(c => c
    //    .StaticFilter(e => e.ProfileId == profile.Id)
    //    .Column()
    //    .Header("{Name}")
    //    .Content(s => s.User.Caption)
    //)
    //{
    //    OpenAction = target => Erp.Docs.OpenDocumentAsync(target.User);
    //}
}