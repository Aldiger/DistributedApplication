using StackExchange.Redis;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Worker.Repositories.Redis;

public interface IVoteRepository{
     Task<Vote> RetrieveVote();
}
public class VoteRepository : IVoteRepository
{
    private readonly ConnectionMultiplexer _connection;
    private readonly AppsettingsDto _appsettings;
    public VoteRepository(IConfiguration configuration)
    {
        _appsettings = configuration.Get<AppsettingsDto>();
        OpenConnection(GetIp(_appsettings.RedisConnectionString));
    }
    public async Task<Vote> RetrieveVote()
    {

        if (_connection == null || !_connection.IsConnected)
        {
            OpenConnection(GetIp(_appsettings.RedisConnectionString));
        }

        var db = _connection.GetDatabase();
        var voteJson = await db.ListLeftPopAsync("votes");
        if (voteJson.IsNullOrEmpty)
            return null;
        return JsonConvert.DeserializeObject<Vote>(voteJson);
    }


    private static ConnectionMultiplexer OpenConnection(string hostname)
    {
        // Use IP address to workaround https://github.com/StackExchange/StackExchange.Redis/issues/410
        var ipAddress = GetIp(hostname);
        Console.WriteLine($"Found redis at {ipAddress}");

        while (true)
        {
            try
            {
                Console.Error.WriteLine("Connecting to redis");
                return ConnectionMultiplexer.Connect(ipAddress);
            }
            catch (RedisConnectionException)
            {
                Console.Error.WriteLine("Waiting for redis");
                Thread.Sleep(1000);
            }
        }
    }

    private static string GetIp(string hostname)
           => Dns.GetHostEntryAsync(hostname)
               .Result
               .AddressList
               .First(a => a.AddressFamily == AddressFamily.InterNetwork)
               .ToString();
}