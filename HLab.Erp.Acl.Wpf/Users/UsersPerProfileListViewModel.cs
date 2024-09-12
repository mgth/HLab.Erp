﻿using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using System;
using HLab.Mvvm.Application;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Acl.Users;

public class UsersPerProfileListViewModel : Core.EntityLists.EntityListViewModel<UserProfile>
{
    public UsersPerProfileListViewModel(IAclService acl, IDocumentService docs, Injector i, Profile profile) : base(i, c => c
        .StaticFilter(e => e.ProfileId == profile.Id)
        .Column("Name")
        .Header("{Name}")
        .Content(s => s.User.Caption)
    )
    {
        _acl = acl;
        OpenAction = target => docs.OpenDocumentAsync(target.User);
    }

    readonly IAclService _acl;

    protected override bool AddCanExecute(Action<string> errorAction) => _acl.IsGranted(errorAction, AclRights.ManageProfiles);
    protected override bool DeleteCanExecute(UserProfile profile, Action<string> errorAction) => _acl.IsGranted(errorAction, AclRights.ManageProfiles);
}