using System.Threading.Tasks;
using Postgres = Worker.Repositories.Postgres;
using Redis = Worker.Repositories.Redis;
namespace Worker;

public interface IVoteService
{
    Task Process();
}
public class VoteService : IVoteService
{
    private readonly Redis.IVoteRepository _redisVoteRepository;
    private readonly Postgres.IVoteRepository _postgresVoteRepository;

    public VoteService(
        Redis.IVoteRepository redisVoteRepository,
        Postgres.IVoteRepository postgresRepository
    )
    {
        _redisVoteRepository = redisVoteRepository;
        _postgresVoteRepository = postgresRepository;
    }

    public async Task Process()
    {
        var vote = await _redisVoteRepository.RetrieveVote();

        if(vote is not null)
            _postgresVoteRepository.Save(vote);
    }

}