using System;

namespace HLab.Erp.Data
{
    //public abstract class  ErpContext : DbContext
    //{
    //    public ErpContext()
    //    : base(DbService.D.GetConnection(), false)
    //    {
    //        this.Configuration.LazyLoadingEnabled = false;
    //    }
    //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        var initializer =
    //            new SQLite.CodeFirst.SqliteCreateDatabaseIfNotExists<ErpContext>(modelBuilder);
    //        Database.SetInitializer(initializer);

    //        base.OnModelCreating(modelBuilder);
    //    }




    //    //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    //        {
    //    //            //optionsBuilder.EnableSensitiveDataLogging();
    //    //            //MultipleActiveResultSets=True;
    //    //#if LOCALSQL
    //    //    //optionsBuilder.UseMySql("server = localhost; user id = root; password = maudpace; persistsecurityinfo = True; database = lims; Convert Zero Datetime = True; ");
    //    //    optionsBuilder.UseMySql("server = localhost; user id = root; password = maudpace; persistsecurityinfo = True; database = limsmonographies; Convert Zero Datetime = True; ");
    //    //#else
    //    //            //optionsBuilder.UseMySQL("server = localhost; user id = root; password = maudpace; persistsecurityinfo = True; database = echantillonage; Convert Zero Datetime = True; ");
    //    //            //optionsBuilder.UseMySql("server = localhost; user id = root; password = maudpace; persistsecurityinfo = True; database = echantillonage; Convert Zero Datetime = True; ");
    //    //            //optionsBuilder.UseNpgsql("server = localhost; user id = root; password = maudpace;");
    //    //            //optionsBuilder..UseMySql("server = 192.168.2.3; user id = fabien; password = maudpace; persistsecurityinfo = True; database = limsmonographies; Convert Zero Datetime = True; ");
    //    //#endif

    //    //            optionsBuilder.UseSqlite("Data Source=echantillonage.db;");
    //    //            base.OnConfiguring(optionsBuilder);
    //    //        }
    //    public void Migrate()
    //    {
    //        //this.Database..Migrate();
    //    }

    //}

    [AttributeUsage(AttributeTargets.Class , AllowMultiple = false, Inherited = true)]
    public class SoftIncrementAttribut : Attribute
    {

    }
}
