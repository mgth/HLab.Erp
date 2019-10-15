using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using Microsoft.Win32;
using System;
using System.IO;

namespace HLab.Erp.Core.Wpf
{
    [Export(typeof(IOptionsService)), Singleton]
    public class OptionsServicesWpf : IOptionsService
    {
        public void SetRegistryPath(string registryPath)
        {
            RegistryPath = registryPath;

        }
        public string RegistryPath { get; set; }
        public string GetOptionString(string name)
        {
            using (var rk = Registry.CurrentUser.OpenSubKey(@"Software\" + RegistryPath))
            {
                if (rk == null) return null;
                var s = rk.GetValue(name).ToString();

                return s;
            }
        }
        public StreamReader GetOptionFileReader(string name)
        {
            var fileName = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), RegistryPath + @"\" + RegistryPath);

            return new StreamReader(fileName);
        }
        public StreamWriter GetOptionFileWriter(string name)
        {
            var fileName = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), RegistryPath + @"\" + RegistryPath);

            return new StreamWriter(fileName);
        }
    }
}
