using System;
using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Icons.Avalonia.Icons.Providers;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Avalonia;

/// <summary>
/// Enregistre les icônes stockées en base (table Icon) dans le service d'icônes,
/// notamment les drapeaux des pays (Icon/Country/Flag/{IsoA3}).
/// Porté de HLab.Erp.Base.Wpf.DbIconModule.
/// </summary>
public class DbIconModule(IIconService icons, IDataService data) : Bootloader
{
    public override async Task<BootState> LoadAsync()
    {
        if (data.ServiceState != ServiceState.Available) return BootState.Requeue;
        var dataIcons = data.FetchAsync<Icon>().ConfigureAwait(true);

        try
        {
            await foreach (var icon in dataIcons)
            {
                var path = icon.Path.ToLower();

                // Contrairement au WPF, on privilégie la source SVG : les sources
                // XAML de la base sont du markup WPF que le chargeur Avalonia ne
                // comprend pas.
                if (!string.IsNullOrWhiteSpace(icon.SourceSvg))
                {
                    icons.AddIconProvider(path, new IconProviderSvgFromSource(icon.SourceSvg, path, icon.Foreground));
                }
                else if (!string.IsNullOrWhiteSpace(icon.SourceXaml))
                {
                    icons.AddIconProvider(path, new IconProviderXamlFromSource(icon.SourceXaml, path, icon.Foreground));
                }
            }
        }
        catch (DataException)
        {
        }

        return BootState.Completed;
    }
}
