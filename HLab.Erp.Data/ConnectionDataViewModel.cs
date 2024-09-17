using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Mvvm.ReactiveUI;
using HLab.Network;
using ReactiveUI;

namespace HLab.Erp.Data;

public class ConnectionDataViewModel : ViewModel<ConnectionData>
{
    readonly IDataService _data;
    public ConnectionDataViewModel(IDataService data, IpScanner scanner)
    {
        _data = data;
        Scanner = scanner;

        Scanner.Scan(5432);

        OkCommand = ReactiveCommand.Create(() => {});

        this.WhenAnyValue(
            e => e.Model.Server,
            e => e.Model.UserName,
            e => e.Model.Password
            )
            .Select(async c => await GetDatabases(c.Item1, c.Item2, c.Item3))
            .Subscribe();
    }

    public async Task GetDatabases(string server, string username, string password)
    {
        if(string.IsNullOrWhiteSpace(server)) return;
        Databases.Clear();
        await foreach (var database in _data.GetDatabasesAsync(server, username, password))
        {
            Databases.Add(database);
        }
    }
    public ObservableCollection<string> Databases { get; } = [];

    IpScanner Scanner { get; }

    public ReadOnlyObservableCollection<string> Servers => Scanner.FoundServers;

    public ICommand OkCommand { get; }
}