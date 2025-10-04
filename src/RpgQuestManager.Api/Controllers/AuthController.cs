using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    public record RegisterRequest(string Username, string Email, string Password);
    public record LoginRequest(string Username, string Password);
    public record TokenRequest(string Token);

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Validação manual
            if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 3)
            {
                return BadRequest(new { message = "Nome de usuário deve ter pelo menos 3 caracteres" });
            }
            
            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
            {
                return BadRequest(new { message = "Email inválido" });
            }
            
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            {
                return BadRequest(new { message = "Senha deve ter pelo menos 6 caracteres" });
            }

            var user = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
            if (user == null)
            {
                return BadRequest(new { message = "Usuário já existe ou dados inválidos" });
            }

            var token = await _authService.GenerateTokenAsync(user);
            return Ok(new
            {
                message = "Usuário registrado com sucesso",
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    createdAt = user.CreatedAt
                },
                token
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _authService.LoginAsync(request.Username, request.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Credenciais inválidas" });
            }

            var token = await _authService.GenerateTokenAsync(user);
            return Ok(new
            {
                message = "Login realizado com sucesso",
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    lastLoginAt = user.LastLoginAt
                },
                token
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("validate")]
    public async Task<IActionResult> ValidateToken()
    {
        try
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Token não fornecido" });
            }

            var user = await _authService.ValidateTokenAsync(token);
            if (user == null)
            {
                return Unauthorized(new { message = "Token inválido ou expirado" });
            }

            return Ok(new
            {
                message = "Token válido",
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] TokenRequest request)
    {
        try
        {
            await _authService.LogoutAsync(request.Token);
            return Ok(new { message = "Logout realizado com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll([FromBody] TokenRequest request)
    {
        try
        {
            var user = await _authService.ValidateTokenAsync(request.Token);
            if (user == null)
            {
                return Unauthorized(new { message = "Token inválido" });
            }

            await _authService.LogoutAllSessionsAsync(user.Id);
            return Ok(new { message = "Todas as sessões foram encerradas" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }
}
