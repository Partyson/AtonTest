using AtonTest.Dto;
using AtonTest.Entities;
using ErrorOr;

namespace AtonTest.Interfaces;

public interface IUsersService
{
    Task<Guid> CreateUser(CreateUserDto user, string loginCreatedBy);
    
    Task<Guid> ModifyUserLogin(string newLogin, string modifiedUserLogin, string loginModifiedBy);
    Task<Guid> ModifyUserPassword(string newPassword, string modifiedUserLogin, string loginModifiedBy);
    Task<Guid> ModifyUserInfo(UserInfoDto user, string modifiedUserLogin, string loginModifiedBy);
    Task<List<UserResponseDto>> GetAllActiveUsers();
    Task<UserResponseByLoginDto?> GetUserByLogin(string login);
    Task<UserResponseByLoginDto?> GetUserByLoginAndPassword(string login, string password);
    Task<List<UserResponseDto>> GetUsersOlderThan(int age);
    
    Task<Guid> DeleteUserByLogin(string login, string loginRevokedBy);
    
    Task<Guid> RestoreUser(string login, string loginRestoredBy);
    
    Task<string?> LoginUser(CredentialsDto credentials);
}