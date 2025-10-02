using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.DTOs.Auth;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Tentativa de registro para usuário: {Username}", request.Username);
        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }
    
    /// <summary>
    /// Autentica um usuário existente
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Tentativa de login para usuário: {Username}", request.Username);
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }
}

