namespace Worker.Tests;

public class VoteServiceTests
{
    private readonly VoteService _voteService;
    private readonly Mock<Redis.IVoteRepository> _redisVoteRepository;
    private readonly Mock<Postgres.IVoteRepository> _postgresVoteRepository;

    public VoteServiceTests()
    {
        _redisVoteRepository= new Mock<Redis.IVoteRepository>();
        _postgresVoteRepository = new Mock<Postgres.IVoteRepository>();

        _voteService= new VoteService(_redisVoteRepository.Object, _postgresVoteRepository.Object);
    }

    [Fact]
    public void ShouldProcessVotesSucessfully()
    {
        //Arrange
        

        //Act
        _voteService.Process();

        //Assert
        Assert.True(true);

    }
}