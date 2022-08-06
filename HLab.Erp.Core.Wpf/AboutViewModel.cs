using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf
{
    public class AboutViewModel : NotifierBase
    {
        #region Constructors

        public AboutViewModel()
        {
            H<AboutViewModel>.Initialize(this);
        }

        #endregion
        #region Properties

        public string Note
        {
            get => _note.Get();
            set => _note.Set(value);
        }

        readonly IProperty<string> _note = H<AboutViewModel>.Property<string>();
        
        #endregion
        #region Data


        #endregion
    }
}
