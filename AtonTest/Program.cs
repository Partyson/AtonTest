using AtonTest.DataContext;
using AtonTest.Helpers;
using AtonTest.Interfaces;
using AtonTest.Repositories;
using AtonTest.Services;
using AtonTest.Validators;
using EntityFrameworkCore.UnitOfWork.Extensions;
using EntityFrameworkCore.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

builder.Services.AddScoped<UserInfoValidator>();
builder.Services.AddScoped<LoginValidator>();
builder.Services.AddScoped<PasswordValidator>();
builder.Services.AddScoped<UserDataValidator>();
builder.Services.AddScoped<ILoginUniquenessValidator, LoginUniquenessValidator>();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<DbContext>(provider => provider.GetService<AppDbContext>()!);
builder.Services.AddUnitOfWork();
builder.Services.AddUnitOfWork<AppDbContext>();

MappingConfig.RegisterMappings();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey("supermegasigmaultrapupersecretkey"u8.ToArray())
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["token"];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    var userService = scope.ServiceProvider.GetRequiredService<IUsersService>();
    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    await AdminCreator.SeedAdminUserAsync(userService, unitOfWork);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();