using Avalonia.Controls;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Avalonia.EntityViews;

/// <summary>
/// Logique d'interaction pour EntityViewDefault.axaml
/// </summary>
public partial class EntityViewDefault : UserControl, IView<DefaultViewMode, IEntity>
{
    public EntityViewDefault()
    {
        InitializeComponent();
    }
}
