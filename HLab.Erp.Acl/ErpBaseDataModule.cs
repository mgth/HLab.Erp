using HLab.Erp.Base.Data;
using HLab.Erp.Data;

namespace HLab.Erp.Acl;

public class ErpBaseDataModule(IDataService data) : DataUpdaterBootloader(data)
{
    protected override ISqlBuilder GetSqlUpdater(string version, ISqlBuilder builder)
    {
        switch (version)
        {
            case "0.0.0.0":
                return builder
                    .Table<AclRight>()
                        .AddColumn(i => i.Description)
                    .Include(base.GetSqlUpdater)
                    .Version("2.1.0.0");
            case "2.1.0.0":
                return builder
                    .Table<Profile>()
                        .AddColumn(i => i.IconPath)
                    .Version("2.2.0.0");

            default:
                break;
        }

        return builder.Include(base.GetSqlUpdater);
    }
}
