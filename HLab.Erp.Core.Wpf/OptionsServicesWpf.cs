using System;
using System.IO;
using System.Threading.Tasks;
using HLab.Core;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using Microsoft.Win32;

namespace HLab.Erp.Core
{
    [Export(typeof(IOptionsService)), Singleton]
    public class OptionsServicesWpf : Service, IOptionsService
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

        public void SetDataService(IService data) => _data = data as IDataService;
        private IDataService _data;

        public async Task<T> GetValue<T>(string name, int? userid, Func<T> defaultValue = null)
        {
            var o = await _data.FetchOneAsync<Option>(e => e.UserId==userid && e.Name==name);
            if (o == null)
                o = await _data.FetchOneAsync<Option>(e => e.UserId == null && e.Name == name);

            if (o == null)
            {
                if (defaultValue != null) return defaultValue();

                return default(T);
            }


            if (typeof(T) == typeof(string))
                return (T)(object)o.Value;

            if (typeof(T) == typeof(double))
                if (double.TryParse(o.Value, out var value))
                {
                    return (T)(object)value;
                }
            if (typeof(T) == typeof(int))
                if (int.TryParse(o.Value, out var value))
                {
                    return (T)(object)value;
                }

            if (typeof(T).IsEnum)
                if (Enum.TryParse(typeof(T),o.Value, out var value))
                {
                    return (T)value;
                }

            return default(T);
        }

        public void SetValue<T>(string name, T value, int? userId)
        {
            var o = _data.FetchOne<Option>(e => e.Name == name && e.UserId == userId);

            if (o == null)
            {
                 o = _data.Add<Option>(e =>
                 {
                     e.Name = name;
                     e.UserId = userId;
                     e.Value = value.ToString();
                 });

                return;
            }

            o.Value = value.ToString();
            _data.Save(o);
        }

        public Task SetValueAsync<T>(string name, T value, int? userid)
        {
            throw new NotImplementedException();
        }
    }
}
