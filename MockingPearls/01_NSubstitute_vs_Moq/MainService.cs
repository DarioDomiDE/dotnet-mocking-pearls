namespace MockingPearls._01_NSubstitute_vs_Moq;

public interface IUserService
{
    string GetUsername(int userId);
}

public class MainService
{
    private readonly IUserService _service;

    public MainService(IUserService service)
    {
        _service = service;
    }

    public string BuildUser(int userId)
    {
        var result = _service.GetUsername(userId);
        result += " is super";
        return result;
    }
}