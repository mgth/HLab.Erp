using HLab.Erp.Data;

namespace HLab.Erp.Base.Data;

public class ErpBaseDataModule(IDataService data) : DataUpdaterBootloader(data)
{
    protected override ISqlBuilder GetSqlUpdater(string version, ISqlBuilder builder)
    {
        switch (version)
        {
            case "0.0.0.0":
                return builder
                    .Table<Icon>()
                        .AddColumn(i => i.Foreground)
                    .Include(base.GetSqlUpdater)
                    .Version("2.0.0.0");

            case "2.0.0.0":
                return builder
                    .Table<Continent>()
                        .AddColumn(c => c.Code)
                        .AlterColumn(c => c.Name)
                    .Include(base.GetSqlUpdater)
                    .Version("2.1.0.0");

            case "2.1.0.0":
                return builder
                    .Table<UnitClass>().Create()
                    .Table<Unit>().Create()
                    .Include(base.GetSqlUpdater)
                    //.Version("2.1.0.0")
                    ;

            default:
                break;
        }

        return builder.Include(base.GetSqlUpdater);
    }
}
