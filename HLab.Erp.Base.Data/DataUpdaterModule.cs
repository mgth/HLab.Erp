using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using Npgsql;

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

                if (version.Version == CurrentVersion)
                {
                    #if DEBUG
                    Upgrade(version.Version);
                    #endif
                    return;
                }

                oldVersion = version.Version;

                Upgrade(version.Version);
            }
        }

        private void Upgrade(string version)
        {
            if (Data is not DataService data) return;
            
            ISqlBuilder builder = new SqlBuilderPostgres(CurrentModule,CurrentVersion);

            GetSqlUpdater(version, builder);

            data.ExecuteSql($"BEGIN;\n{builder}\nCOMMIT;");
        }

        protected virtual ISqlBuilder GetSqlUpdater(string version, ISqlBuilder builder) 
            => builder.SqlResource($"{CurrentModule}.SQL.Update-From-{version}.sql");

        private void CreateDataVersion()
        {
            if (Data is DataService data)
            {
                data.ExecuteSql($"BEGIN;\n{new SqlBuilderPostgres(CurrentModule,CurrentVersion).Table<DataVersion>().Create()}\nCOMMIT;");
            }
        }



    }
}