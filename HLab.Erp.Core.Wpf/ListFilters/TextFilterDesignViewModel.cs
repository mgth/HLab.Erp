﻿using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.ListFilters
{
    public class TextFilterDesignViewModel : TextFilter, IViewModelDesign
    {
        public TextFilterDesignViewModel()
        {
            Value = "DummySearch";
        }
    }
}