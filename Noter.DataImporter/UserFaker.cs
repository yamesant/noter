using Bogus;
using Microsoft.AspNetCore.Identity;
using Noter.Domain;

namespace Noter.DataImporter;

public sealed class UserFaker : Faker<User>
{
    private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
    public UserFaker()
    {
        StrictMode(true);

        RuleFor(u => u.Id, f => f.Random.Guid().ToString());
        RuleFor(u => u.UserName, f => f.UniqueIndex + f.Internet.Email());
        RuleFor(u => u.NormalizedUserName, (_, u) => u.UserName!.ToUpper());
        RuleFor(u => u.Email, (_, u) => u.UserName);
        RuleFor(u => u.NormalizedEmail, (_, u) => u.NormalizedUserName);
        RuleFor(u => u.EmailConfirmed, _ => true);
        RuleFor(u => u.PasswordHash, (_, u) => _passwordHasher.HashPassword(u, $"{u.UserName}Pass#123"));
        RuleFor(u => u.SecurityStamp, f => f.Random.Guid().ToString());
        RuleFor(u => u.ConcurrencyStamp, f => f.Random.Guid().ToString());
        RuleFor(u => u.PhoneNumber, _ => null);
        RuleFor(u => u.PhoneNumberConfirmed, _ => false);
        RuleFor(u => u.TwoFactorEnabled, _ => false);
        RuleFor(u => u.LockoutEnd, _ => null);
        RuleFor(u => u.LockoutEnabled, _ => true);
        RuleFor(u => u.AccessFailedCount, _ => 0);
        
        // Navigation properties
        RuleFor(u => u.Notes, _ => null!);
    }
}