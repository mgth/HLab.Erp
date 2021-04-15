using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Npgsql.Replication.PgOutput.Messages;
using Org.BouncyCastle.Crypto.Parameters;

namespace HLab.Erp.Base.Data
{
    public class ErpBaseDataModule : DataUpdaterModule
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
            }

            return builder.Include(base.GetSqlUpdater);
        }
    }
}
