using System.Timers;
using Timer = System.Timers.Timer;

namespace Flowsy.Web.Api.Security.Clients;

public class ApiClientCache
{
    private readonly IDictionary<string, ApiClient> _clients = new Dictionary<string, ApiClient>();
    private TimeSpan _lifetime;
    private Timer? _timer;

    public ApiClientCache() : this(TimeSpan.Zero)
    {
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs args)
    {
        Clear();
    }

    public ApiClientCache(TimeSpan lifetime)
    {
        Lifetime = lifetime;
    }

    public void Save(params ApiClient[] clients)
    {
        foreach (var client in clients)
            _clients[client.ClientId] = client;
    }

    public IEnumerable<ApiClient> GetAll()
    {
        return _clients.Values;
    }

    public ApiClient? GetOne(string clientId)
        => _clients.ContainsKey(clientId) ? _clients[clientId] : null;

    public void Clear()
    {
        _clients.Clear();
    }

    public TimeSpan Lifetime
    {
        get => _lifetime;
        set
        {
            _lifetime = value;
            _timer?.Dispose();
            _timer = null;
            if (value > TimeSpan.Zero)
            {
                _timer = new Timer(value.TotalMilliseconds);
                _timer.Elapsed += OnTimerElapsed;
            }
        }
    } 
}