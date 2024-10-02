using HLab.Base.ReactiveUI;
using ReactiveUI;

namespace HLab.Erp.Data;

public class ConnectionData : ReactiveModel
{
    public ConnectionData()
    {
        _connectionString = this.WhenAnyValue(
            x => x.Server,
            x => x.Database,
            x => x.UserName,
            x => x.Password,
            (server, database, userName, password) => $"Host={server};Username={userName};Password={password};Database={database}"
        ).ToProperty(this, x => x.ConnectionString);
    }

    public string Server { get => _server; set => this.SetAndRaise(ref _server, value); }
    string _server = "";

    public string Database { get => _database; set => this.SetAndRaise(ref _database, value); }
    string _database = "";

    public string UserName { get => _userName; set => this.SetAndRaise(ref _userName, value); }
    string _userName = "";

    public string Password { get => _password; set => this.SetAndRaise(ref _password, value); }
    string _password = "";

    public string ConnectionString => _connectionString.Value;
    private readonly ObservableAsPropertyHelper<string> _connectionString;
}