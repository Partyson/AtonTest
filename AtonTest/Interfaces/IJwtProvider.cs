using AtonTest.Entities;

namespace AtonTest.Interfaces;

public interface IJwtProvider
{
    string? GenerateToken(UserEntity user);
}