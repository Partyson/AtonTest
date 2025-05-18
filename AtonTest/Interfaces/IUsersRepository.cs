using AtonTest.Entities;
using EntityFrameworkCore.Repository.Interfaces;

namespace AtonTest.Interfaces;

public interface IUsersRepository : IRepository<UserEntity>;