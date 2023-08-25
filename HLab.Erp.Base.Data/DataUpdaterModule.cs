using HLab.Core.Annotations;
using HLab.Erp.Data;
using Npgsql;
using System;

namespace HLab.Erp.Base.Data;

public abstract class DataUpdaterBootloader(IDataService data) : IBootloader
{
    protected virtual string CurrentVersion => this.GetType().Assembly.GetName().Version.ToString();
    protected virtual string CurrentModule => this.GetType().Assembly.GetName().Name;

    public void Load(IBootContext bootstrapper)
    {
        if(bootstrapper.WaitingForService(data)) return;

        var oldVersion = "";
        while (true)
        {
            DataVersion version = null;
            try
            {
                version = data.FetchOne<DataVersion>(d => d.Module == CurrentModule);
            }
            catch (DataException exception)
            {
                if (exception.InnerException is PostgresException postgres)
                {
                    if (postgres.SqlState=="42P01" && postgres.MessageText.Contains("DataVersion"))
                    {
                        CreateDataVersion();
                        version = data.FetchOne<DataVersion>(d => d.Module == CurrentModule);
                    }
                }
            }

            version ??= data.Add<DataVersion>(v =>
            {
                v.Module = CurrentModule;
                v.Version = "0.0.0.0";
            });

            if (version.Version == oldVersion) throw new DataException($"Wrong database version {version.Version} but need {CurrentVersion}",null);

            if (version.Version == CurrentVersion)
            {
#if DEBUG
                try
                {
                    Upgrade(version.Version);

                }
                catch (Exception ex)
                {

                }
#endif
                return;
            }

            oldVersion = version.Version;

            Upgrade(version.Version);
        }
    }

    void Upgrade(string version)
    {
        if (data is not DataService d) return;
        
        ISqlBuilder builder = new SqlBuilderPostgres(CurrentModule,CurrentVersion);

        GetSqlUpdater(version, builder);

        d.ExecuteSql($"BEGIN;\n{builder}\nCOMMIT;");
    }

    protected virtual ISqlBuilder GetSqlUpdater(string version, ISqlBuilder builder) 
        => builder.SqlResource($"{CurrentModule}.SQL.Update-From-{version}.sql");

    void CreateDataVersion()
    {
        if (data is not DataService d) return;
        d.ExecuteSql($"BEGIN;\n{new SqlBuilderPostgres(CurrentModule,CurrentVersion).Table<DataVersion>().Create()}\nCOMMIT;");
    }
}