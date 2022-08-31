using System.ComponentModel;

namespace HLab.Erp.Conformity.Annotations
{
    public interface IFormClass
    {
        byte[] Code { get; }
        string Name { get; set; }
    }

    public interface IFormTarget : INotifyPropertyChanged
    {
        IFormClass FormClass { get; set; }
        string Name { get; set; }

        byte[] Code { get; }

        string SpecificationValues { get; set; }
        bool SpecificationDone { get; set; }

        string ResultValues { get; set; }
        bool MandatoryDone { get; set; }

        string DefaultTestName { get; }
        string TestName { get; set; }
        string Description { get; set; }
        string Specification { get; set; }
        string Conformity { get; set; }
        string Result { get; set; }
        ConformityState ConformityId { get; set; }
    }
}