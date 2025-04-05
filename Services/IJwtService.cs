using HackStack___Gemini.Models;

namespace HackStack___Gemini.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
