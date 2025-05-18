using AtonTest.Dto;
using AtonTest.Entities;
using AtonTest.Interfaces;
using ErrorOr;
using Mapster;

namespace AtonTest.Services;

public class UsersService : IUsersService
{
    private readonly IUsersRepository usersRepository;
    private readonly IJwtProvider jwtProvider;

    public UsersService(IUsersRepository usersRepository, IJwtProvider jwtProvider)
    {
        this.usersRepository = usersRepository;
        this.jwtProvider = jwtProvider;
    }

    public async Task<Guid> CreateUser(CreateUserDto user, string loginCreatedBy)
    {
        var newUserEntity = user.Adapt<UserEntity>();
        newUserEntity.SetCreated(loginCreatedBy);
        var addedUser = await usersRepository.AddAsync(newUserEntity);
        return addedUser.Id;
    }

    public async Task<Guid> ModifyUserInfo(UserInfoDto user, string modifiedUserLogin, string loginModifiedBy)
    {
        var userEntity = await GetUserEntityByLogin(modifiedUserLogin);
        if (userEntity == null)
            return Guid.Empty;

        user.Adapt(userEntity);
        userEntity.SetModified(loginModifiedBy);
        return userEntity.Id;
    }
    
    public async Task<Guid> ModifyUserPassword(string newPassword, string modifiedUserLogin, string loginModifiedBy)
    {
        var userEntity = await GetUserEntityByLogin(modifiedUserLogin);
        if (userEntity == null)
            return Guid.Empty;

        userEntity.Password = newPassword;
        userEntity.SetModified(loginModifiedBy);
        return userEntity.Id;
    }

    public async Task<Guid> ModifyUserLogin(string newLogin, string modifiedUserLogin, string loginModifiedBy)
    {
        var userEntity = await GetUserEntityByLogin(modifiedUserLogin);
        if (userEntity == null)
            return Guid.Empty;

        userEntity.Login = newLogin;
        userEntity.SetModified(loginModifiedBy);
        return userEntity.Id;
    }

    public async Task<List<UserResponseDto>> GetAllActiveUsers()
    {
        var query = usersRepository.MultipleResultQuery().AndFilter(x => x.RevokedOn == null);
        var userEntities = await usersRepository.SearchAsync(query);
        return userEntities.OrderBy(x => x.CreatedOn)
            .Select(x => x.Adapt<UserResponseDto>()).ToList();
    }

    public async Task<UserResponseByLoginDto?> GetUserByLogin(string login)
    {
        var userEntity = await GetUserEntityByLogin(login);
        return userEntity?.Adapt<UserResponseByLoginDto>();
    }

    public async Task<UserResponseByLoginDto?> GetUserByLoginAndPassword(string login, string password)
    {
        var userEntity = await GetUserEntityByLogin(login);
        if (userEntity != null && userEntity.Password == password)
            return userEntity.Adapt<UserResponseByLoginDto>();
        return null;
    }

    public async Task<List<UserResponseDto>> GetUsersOlderThan(int age)
    {
        var today = DateTime.UtcNow;
        var minBirthDate = today.AddYears(-age - 1).Date;
        var query = usersRepository.MultipleResultQuery()
            .AndFilter(x => x.Birthday != null && x.Birthday <= minBirthDate);
        var userEntities = await usersRepository.SearchAsync(query);
        return userEntities
            .Select(x => x.Adapt<UserResponseDto>()).ToList();
    }

    public async Task<Guid> DeleteUserByLogin(string login, string loginRevokedBy)
    {
        var userEntity = await GetUserEntityByLogin(login);
        if (userEntity == null || userEntity.RevokedOn != null)
            return Guid.Empty;
        userEntity.SetRevoked(loginRevokedBy);
        return userEntity.Id;
    }

    public async Task<Guid> RestoreUser(string login, string loginRestoredBy)
    {
        var userEntity = await GetUserEntityByLogin(login);
        if (userEntity == null)
            return Guid.Empty;
        userEntity.SetRestored(loginRestoredBy);
        return userEntity.Id;
    }

    public async Task<string?> LoginUser(CredentialsDto credentials)
    {
        var userEntity = await GetUserEntityByLogin(credentials.Login);
        if (userEntity != null && userEntity.Password == credentials.Password)
            return jwtProvider.GenerateToken(userEntity);
        return null;
    }
    
    private async Task<UserEntity?> GetUserEntityByLogin(string login)
    {
        var query = usersRepository.SingleResultQuery()
            .AndFilter(x => x.Login == login);
        var userEntity = await usersRepository.FirstOrDefaultAsync(query);
        return userEntity;
    }
}