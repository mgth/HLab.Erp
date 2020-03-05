using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;

namespace HLab.Erp.Data
{
    public  class ErpDataBootloader : IBootloader
    {
        private readonly IDataService _db;
        private readonly IExportLocatorScope _container;
        private readonly IOptionsService _opt;

        [Import] public ErpDataBootloader(IDataService db, IExportLocatorScope container, IOptionsService opt)
        {
            _db = db;
            _container = container;
            _opt = opt;
        }

        public bool Load()
        {
            _db.RegisterEntities(_container);
            var connectionString = _opt.GetOptionString("Connection");
            //var driver = _opt.GetOptionString("Driver");

            _db.Register(connectionString,"");
            return true;
        }
    }
}
