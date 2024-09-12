﻿using System.Windows.Controls;
using HLab.Erp.Core.Tools.Details;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Acl.Profiles;

/// <summary>
/// Logique d'interaction pour ProfileView.xaml
/// </summary>
public partial class ProfileView : UserControl, IView<ProfileViewModel>, IDocumentViewClass, IDetailViewClass
{
    public ProfileView()
    {
        InitializeComponent();
    }
}