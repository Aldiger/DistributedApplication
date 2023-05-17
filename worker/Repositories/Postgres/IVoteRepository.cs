using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data.Common;
using System.Net.Sockets;

namespace Worker.Repositories.Postgres
{
    public interface IVoteRepository{
        void Save(Vote vote);
    }

    public class VoteRepository: IVoteRepository{

        private readonly AppsettingsDto _settings;

        private NpgsqlConnection _connection;

        public VoteRepository(IConfiguration configuration)
        {
            _settings = configuration.Get<AppsettingsDto>();

            OpenConnection();
        }


        public void Save(Vote vote)
        {
            if (!_connection.State.Equals(System.Data.ConnectionState.Open))
            {
                OpenConnection();
            }

            var command  = _connection.CreateCommand();
            try
            {
                command.CommandText = @"INSERT INTO votes (id, vote) VALUES (@id, @vote)";
                command.Parameters.AddWithValue("@id", vote.voter_id);
                command.Parameters.AddWithValue("@vote", vote.vote);
                command.ExecuteNonQuery();

            }
            catch (DbException)
            {
                //update vote
                command.CommandText = @"UPDATE votes SET vote = @vote WHERE id = @id";
                command.ExecuteNonQuery();
            }
            finally
            {
                command.Dispose();
            }
         
        }
        private void OpenConnection()
        {
            while (true)
            {
                try
                {
                    _connection = new NpgsqlConnection(_settings.PostgresConnectionString);
                    _connection.Open();
                    break;

                }
                catch (SocketException)
                {
                    Console.WriteLine("Waiting for Postgres to start...");
                    System.Threading.Thread.Sleep(1000);
                }
                catch (DbException)
                {
                    Console.WriteLine("Waiting for Postgres to start...");
                    System.Threading.Thread.Sleep(1000);
                } 
            }

            var command = _connection.CreateCommand();
            command.CommandText = @"CREATE TABLE IF NOT EXISTS votes (
                                    id VARCHAR(255) NOT NULL UNIQUE,
                                    vote VARCHAR(255) NOT NULL
                                )";
            command.ExecuteNonQuery();
        }
    }

}
