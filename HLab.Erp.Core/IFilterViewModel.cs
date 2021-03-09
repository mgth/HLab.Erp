namespace HLab.Erp.Core
{
    public interface IFilterViewModel
    {
        bool Enabled { get; set; }
        string Title { get; set; }
        string IconPath { get; set; }
    }
}
