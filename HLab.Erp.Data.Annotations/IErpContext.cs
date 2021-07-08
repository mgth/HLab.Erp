namespace HLab.Erp.Data
{
    public interface IErpContext
    {
        IDataService Db { get; }
    }
}
