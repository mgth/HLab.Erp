using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using Npgsql;
using Org.BouncyCastle.Crypto.Parameters;

namespace HLab.Erp.Base.Data
{
    public abstract class DataUpdaterModule : IBootloader
    {
        [Import] public  IDataService Data { get; set; }

        protected virtual string CurrentVersion => this.GetType().Assembly.GetName().Version.ToString();
        protected virtual string CurrentModule => this.GetType().Assembly.GetName().Name;

        public void Load(IBootContext bootstrapper)
        {
            string oldVersion = "";
            while (true)
            {
                DataVersion version = null;
                try
                {
                    version = Data.FetchOne<DataVersion>(d => d.Module == CurrentModule);
                }
                catch (DataException exception)
                {
                    if (exception.InnerException is PostgresException postgres)
                    {
                        if (postgres.SqlState=="42P01" && postgres.MessageText.Contains("DataVersion"))
                        {
                            CreateDataVersion();
                            version = Data.FetchOne<DataVersion>(d => d.Module == CurrentModule);
                        }
                    }
                }

                if (version == null)
                {
                    version = Data.Add<DataVersion>(v =>
                    {
                        v.Module = CurrentModule;
                        v.Version = "0.0.0.0";
                    });
                }

                if (version.Version == oldVersion) throw new DataException($"Wrong database version {version.Version} but need {CurrentVersion}",null);

                if (version.Version == CurrentVersion) return;

                oldVersion = version.Version;

                using (var transaction = Data.GetTransaction())
                {
                    Upgrade(version.Version, transaction);
                }

            }
        }

        protected virtual void Upgrade(string version, IDataTransaction dbTransaction)
        {
            var sql = GetSqlFile($"Update-From-{version}");
            if (string.IsNullOrWhiteSpace(sql)) return;

            dbTransaction.ExecuteSql(sql);

        }

        private void CreateDataVersion()
        {
            if (Data is DataService data)
            {
                data.CreateTable<DataVersion>();
            }
        }

        protected string GetSqlFile(string fileName)
        {

            var thisAssembly = Assembly.GetExecutingAssembly();
            using var s = thisAssembly.GetManifestResourceStream(
                $"{CurrentModule}.SQL.{fileName}.sql");
            if (s == null) return null;

            using var sr = new StreamReader(s);
            return sr.ReadToEnd();

        }
    }

    public class ErpBaseDataModule : DataUpdaterModule
    {
    }
}
