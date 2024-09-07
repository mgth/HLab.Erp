using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntitySelectors;

namespace HLab.Erp.Base.Countries;

public class CountryForeignViewModel(EntityViewModel<Country>.Injector i) : ForeignViewModel<Country>(i);