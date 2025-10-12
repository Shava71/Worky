namespace AuthService.Application;

public class LoginResponse
{
    public string Id { get; set; }
    public string Token { get; set; }
    public IList<string> Role { get; set; }
}