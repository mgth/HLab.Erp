namespace HLab.Erp.Acl
{
    public interface IAclTarget
    {
        IAclTarget Parent { get; }        
        string AclTargetId { get; }
    }
}
