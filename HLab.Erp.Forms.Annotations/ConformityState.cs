namespace HLab.Erp.Conformity.Annotations
{ 
    public enum ConformityState
    {
        Undefined = -1,
        NotChecked = 0,
        Running = 1,
        NotConform = 2,
        Conform = 3,
        Invalid = 4,
    }

    public static class ConformityExtension
    {
        public static string IconPath(this ConformityState state) => $"Icons/Conformity/{state}";
        public static string Caption(this ConformityState state) => $"{{{state}}}";
    }

}