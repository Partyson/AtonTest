using AtonTest.DataContext;
using AtonTest.Entities;
using AtonTest.Interfaces;
using EntityFrameworkCore.Repository;

namespace AtonTest.Repositories;

public class UsersRepository(AppDbContext context) : Repository<UserEntity>(context), IUsersRepository;