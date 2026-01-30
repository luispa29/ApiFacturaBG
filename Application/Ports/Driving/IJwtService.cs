namespace Application.Ports.Driving
{
    public interface IJwtService
    {
        string GenerateToken(int userId);
        int? ValidateTokenAndGetUserId(string token);
    }
}
