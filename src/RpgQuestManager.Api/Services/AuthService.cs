using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RpgQuestManager.Api.Services;

public class AuthService
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<User?> RegisterAsync(string username, string email, string password)
    {
        // Verificar se usuário já existe
        if (await _db.Users.AnyAsync(u => u.Username == username || u.Email == email))
            return null;

        // Criar hash da senha
        var passwordHash = HashPassword(password);

        // Criar usuário
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return user;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return null;

        // Atualizar último login
        user.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return user;
    }

    public async Task<string> GenerateTokenAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "default-key-that-should-be-changed-in-production");
        var issuer = _configuration["Jwt:Issuer"] ?? "RpgQuestManager";
        var audience = _configuration["Jwt:Audience"] ?? "RpgQuestManager";
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Issuer = issuer,
            Audience = audience,
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // Salvar sessão
        var session = new UserSession
        {
            UserId = user.Id,
            Token = tokenString,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsActive = true
        };

        _db.UserSessions.Add(session);
        await _db.SaveChangesAsync();

        return tokenString;
    }

    public async Task<User?> ValidateTokenAsync(string token)
    {
        var session = await _db.UserSessions
            .Include(us => us.User)
            .FirstOrDefaultAsync(us => us.Token == token && us.IsActive && us.ExpiresAt > DateTime.UtcNow);

        return session?.User;
    }

    public async Task LogoutAsync(string token)
    {
        var session = await _db.UserSessions.FirstOrDefaultAsync(us => us.Token == token);
        if (session != null)
        {
            session.IsActive = false;
            await _db.SaveChangesAsync();
        }
    }

    public async Task LogoutAllSessionsAsync(int userId)
    {
        var sessions = await _db.UserSessions.Where(us => us.UserId == userId && us.IsActive).ToListAsync();
        foreach (var session in sessions)
        {
            session.IsActive = false;
        }
        await _db.SaveChangesAsync();
    }

    private string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[16];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        var hashBytes = new byte[48];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            var hashBytes = Convert.FromBase64String(passwordHash);
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}
