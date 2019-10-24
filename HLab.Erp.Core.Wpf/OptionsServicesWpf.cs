using System;
using System.IO;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using Microsoft.Win32;

namespace HLab.Erp.Core
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
                Environment.SpecialFolder.ApplicationData), RegistryPath + @"\" + name);

            return new StreamReader(fileName);
        }
        public StreamWriter GetOptionFileWriter(string name)
        {
            var fileName = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), RegistryPath + @"\" + name);

            var dir = Path.GetDirectoryName(fileName);
            Directory.CreateDirectory(dir);

            return new StreamWriter(fileName);
        }
    }
}
