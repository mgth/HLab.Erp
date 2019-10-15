#define NPOCO
using System.Data.Common;
using HLab.DependencyInjection.Annotations;
#if NPOCO
using NPoco;
#endif

namespace HLab.Erp.Data
{
#if NPOCO
    public class ErpContext : Database
    {
        public ErpContext(DbConnection connection) : base(connection)
        {
        }

        public ErpContext(DbConnection connection, DatabaseType dbType) : base(connection, dbType)
        {
        }

        public ErpContext(string connectionString, DatabaseType databaseType, DbProviderFactory provider) : base(connectionString, databaseType, provider)
        {
        }

    }
#endif

#if EFCORE
    public class ErpContext : DbContext, IErpContext
    {

        public ErpContext(IDataService db)
        {
            Db = db;
        }
        public IDataService Db { get; }

        [Import]
        public IExportLocatorScope Container { get; private set; }

 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //TODO : connectionString est null quand rien dans la base de registre
            //Prévoir un message d'erreur, plutot que l'exeption

            optionsBuilder
#if MYSQL
                .UseMySQL(Db.ConnectionString);
#endif
                .UseNpgsql(Db.ConnectionString);
            //.ReplaceService<IEntityMaterializerSource, ErpEntityMaterializerSource>()
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var type in Db.Entities)
            {
                modelBuilder.Entity(type);
            }
        }
    }
#endif
}
