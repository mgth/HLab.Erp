using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using HLab.ColorTools.Wpf;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Icons.Annotations.Icons;
using HLab.Icons.Wpf.Icons;
using HLab.Icons.Wpf.Icons.Providers;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf.Entities.Icons
{
    using H = H<IconViewModel>;

    public class IconViewModel : ListableEntityViewModel<Icon>
    {
        readonly IIconService _icons;
        public IconViewModel(IIconService icons, Injector i):base(i)
        {
            _icons = icons;
            H.Initialize(this);
        }

        public object Icon => _icon.Get();

        readonly IProperty<object> _icon = H.Property<object>(c => c
            .Set(async e => (object)XamlTools.SetForeground(
                (UIElement)await XamlTools.FromXamlStringAsync(e.Model.SourceXaml),
                e.Model.Foreground.ToColor(),Brushes.White)
            )
            .On(e => e.Model.SourceXaml)
            .Update()
        );

        public ICommand ToXamlCommand { get; } = H.Command(c => c
            .Action(e => e.ToXamlAsync())
        );
        public ICommand EditSvgCommand { get; } = H.Command(c => c
            .Action(async e => await e.EditSvgAsync().ConfigureAwait(true))
        );

        async Task ToXamlAsync()
        {
            Model.SourceXaml = await XamlTools.SvgToXamlAsync(Model.SourceSvg).ConfigureAwait(false);
        }

        async Task EditSvgAsync()
        {
            var fileName = Path.GetTempPath() + Guid.NewGuid() + ".svg";

            var app = AssocQueryString(AssocStr.Executable, ".svg");

            await File.WriteAllTextAsync(fileName,Model.SourceSvg).ConfigureAwait(true);

            var p = Process.Start(app,fileName);

            if (p != null)
            {
                await p?.WaitForExitAsync();       
                
                await LoadSvgAsync(fileName).ConfigureAwait(false);

            }
            //pEditor.StartInfo.Arguments = ""; 
        }

        ITrigger _onSave = H.Trigger(c => c
            .On(e => e.Locker.IsActive)
            .Do(e => e.UpdateIcon())
        );

        void UpdateIcon()
        {
            if(Locker.IsActive) return;

            var path = Model.Path.ToLower();

            if (!string.IsNullOrWhiteSpace(Model.SourceXaml))
            {
                _icons.AddIconProvider(path, new IconProviderXamlFromSource(Model.SourceXaml, path, Model.Foreground));
            }
            else if (!string.IsNullOrWhiteSpace(Model.SourceSvg))
            {
                _icons.AddIconProvider(path, new IconProviderSvgFromSource(Model.SourceSvg, path, Model.Foreground));
            }
        }

        async Task LoadSvgAsync(string path)
        {
            Model.SourceSvg = await File.ReadAllTextAsync(path).ConfigureAwait(true);
            await ToXamlAsync();
        }
        static string AssocQueryString(AssocStr association, string extension)
        {
            const int S_OK = 0;
            const int S_FALSE = 1;

            uint length = 0;
            uint ret = AssocQueryString(AssocF.None, association, extension, null, null, ref length);
            if (ret != S_FALSE)
            {
                throw new InvalidOperationException("Could not determine associated string");
            }

            var sb = new StringBuilder((int)length); // (length-1) will probably work too as the marshaller adds null termination
            ret = AssocQueryString(AssocF.None, association, extension, null, sb, ref length);
            if (ret != S_OK)
            {
                throw new InvalidOperationException("Could not determine associated string"); 
            }

            return sb.ToString();
        }
        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern uint AssocQueryString(
        AssocF flags, 
        AssocStr str,  
        string pszAssoc, 
        string pszExtra, 
        [Out] StringBuilder pszOut, 
        ref uint pcchOut
        ); 

        [Flags]
        public enum AssocF
        {
            None = 0,
            Init_NoRemapCLSID = 0x1,
            Init_ByExeName = 0x2,
            Open_ByExeName = 0x2,
            Init_DefaultToStar = 0x4,
            Init_DefaultToFolder = 0x8,
            NoUserSettings = 0x10,
            NoTruncate = 0x20,
            Verify = 0x40,
            RemapRunDll = 0x80,
            NoFixUps = 0x100,
            IgnoreBaseClass = 0x200,
            Init_IgnoreUnknown = 0x400,
            Init_Fixed_ProgId = 0x800,
            Is_Protocol = 0x1000,
            Init_For_File = 0x2000
        }

        public enum AssocStr
        {
            Command = 1,
            Executable,
            FriendlyDocName,
            FriendlyAppName,
            NoOpen,
            ShellNewValue,
            DDECommand,
            DDEIfExec,
            DDEApplication,
            DDETopic,
            InfoTip,
            QuickTip,
            TileInfo,
            ContentType,
            DefaultIcon,
            ShellExtension,
            DropTarget,
            DelegateExecute,
            Supported_Uri_Protocols,
            ProgID,
            AppID,
            AppPublisher,
            AppIconReference,
            Max
        }    }
}