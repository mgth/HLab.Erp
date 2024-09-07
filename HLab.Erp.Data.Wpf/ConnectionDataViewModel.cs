using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Mvvm;
using HLab.Mvvm.ReactiveUI;
using HLab.Network;

namespace HLab.Erp.Data.Wpf;

public class ConnectionDataViewModel : ViewModel<ConnectionData>
{
    readonly IDataService _data;
    public ConnectionDataViewModel(IDataService data, IpScanner scanner)
    {
        _data = data;
        _scanner = scanner;

        _scanner.Scan(5432);

        OkCommand = ReactiveUI.ReactiveCommand.Create(() => {});
    }


    public async Task GetDatabases()
    {
        await foreach (var database in _data.GetDatabasesAsync(Model.Server, Model.UserName, Model.Password))
        {
            Databases.Add(database);
        }
    }

    public ObservableCollection<string> Databases { get; } = new();

    public string Server
    {
        get => _server;
        set

        {
            if (!SetAndRaise(ref _server, value)) return;
            Model.Server = value;
            GetDatabases();
        }
    }

    string _server;

    IpScanner _scanner { get; }

    public ReadOnlyObservableCollection<string> Servers => _scanner.FoundServers;

    public ICommand OkCommand { get; }
}