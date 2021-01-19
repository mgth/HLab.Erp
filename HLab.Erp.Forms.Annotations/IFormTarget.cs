namespace HLab.Erp.Forms.Annotations
{

    public interface IFormTarget
    {
        FormState State { get; set; }
        string SpecValues { get; set; }
        string Values { get; set; }
        bool MandatoryDone { get; set; }
        bool SpecificationsDone { get; set; }
    }
}