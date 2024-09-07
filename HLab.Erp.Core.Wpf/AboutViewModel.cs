using HLab.Mvvm.ReactiveUI;

namespace HLab.Erp.Core.Wpf
{
    public class AboutViewModel : ViewModel
    {
        #region Constructors

        public AboutViewModel()
        {
        }

        #endregion
        #region Properties

        public string Note
        {
            get => _note;
            set => SetAndRaise(ref _note,value);
        }
        string _note;
        
        #endregion
        #region Data


        #endregion
    }
}
