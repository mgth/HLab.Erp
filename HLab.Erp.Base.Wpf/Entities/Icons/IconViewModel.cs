using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Icons.Annotations.Icons;
using HLab.Icons.Wpf;
using HLab.Icons.Wpf.Icons;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf.Entities.Icons
{
    using H = H<IconViewModel>;

    public class IconViewModel : EntityViewModel<Icon>
    {
        public IconViewModel() => H.Initialize(this);

        public override string Title => _title.Get();

        private readonly IProperty<string> _title = H.Property<string>(c => c
            .On(e => e.Model.Caption)
            .Set(e => e.Model.Caption)
        );

        [Import]
        private IIconService _icons;

        public object Icon => _icon.Get();
        private readonly IProperty<object> _icon = H.Property<object>(c => c
            .On(e => e.Model.SourceXaml)
            .Set(e => XamlTools.FromXamlStringAsync(e.Model.SourceXaml))
        );

        public ICommand ToXamlCommand { get; } = H.Command(c => c
            .Action(e => e.ToXaml())
        );
        public ICommand EditSvgCommand { get; } = H.Command(c => c
            .Action(async e => await e.EditSvgAsync().ConfigureAwait(true))
        );

        private async void ToXaml()
        {
            Model.SourceXaml = await XamlTools.SvgToXamlAsync(Model.SourceSvg).ConfigureAwait(false);
        }

        private async Task EditSvgAsync()
        {
            var fileName = Path.GetTempPath() + Guid.NewGuid() + ".svg";

            var app = AssocQueryString(AssocStr.Executable, ".svg");

            await File.WriteAllTextAsync(fileName,Model.SourceSvg).ConfigureAwait(true);

            var p = Process.Start(app,fileName);

            //pEditor.StartInfo.Arguments = ""; 
            p?.WaitForExit();       
            
            await LoadSvgAsync(fileName).ConfigureAwait(false);
        }

        private async Task LoadSvgAsync(string path)
        {
            Model.SourceSvg = await File.ReadAllTextAsync(path).ConfigureAwait(true);
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