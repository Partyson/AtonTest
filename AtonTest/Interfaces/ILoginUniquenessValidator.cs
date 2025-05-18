namespace AtonTest.Interfaces;

public interface ILoginUniquenessValidator
{
    Task<bool> IsLoginUniqueAsync(string login);
}