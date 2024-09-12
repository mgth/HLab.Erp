﻿using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Acl.KioskLogin;

/// <summary>
/// Logique d'interaction pour UserView.xaml
/// </summary>
public partial class UserView : UserControl, IView<DefaultViewMode,User>, IListItemViewClass
{
    public UserView()
    {
        InitializeComponent();
    }
}