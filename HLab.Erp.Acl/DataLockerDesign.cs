using HLab.Mvvm.Annotations;


namespace HLab.Erp.Acl;

public class DataLockerDesign : DataLocker<DataLockerEntityDesign>, IDesignViewModel
{
    public DataLockerDesign() : base(new DataLockerEntityDesign(),null,null,null,null,null)
    {
    }
}