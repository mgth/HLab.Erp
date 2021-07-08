using HLab.Mvvm.Annotations;


namespace HLab.Erp.Acl
{
    public class DataLockerDesign : DataLocker<DataLockerEntityDesign>, IViewModelDesign
    {
        public DataLockerDesign() : base(new DataLockerEntityDesign(),null,null,null,null,null)
        {
        }
    }
}