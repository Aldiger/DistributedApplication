using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Postgres = Worker.Repositories.Postgres;
using Redis = Worker.Repositories.Redis;

namespace Worker
{
    public class Program
    {
        private static IConfiguration _configuration;
        private static IVoteService _voteService;
        public async static Task<int> Main(string[] args)
        {
            RegisterServices();

            try
            {
                while(true){

                    Thread.Sleep(200);
                    await _voteService.Process();
                
                }

            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }
        }

        private static void RegisterServices()
        {
             _configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(Program._configuration)
                .AddSingleton<Redis.IVoteRepository, Redis.VoteRepository>()
                .AddSingleton<Postgres.IVoteRepository, Postgres.VoteRepository>()
                .AddSingleton<IVoteService, VoteService>()
                .BuildServiceProvider();

            _voteService = serviceProvider.GetRequiredService<IVoteService>();

        }
    }
}