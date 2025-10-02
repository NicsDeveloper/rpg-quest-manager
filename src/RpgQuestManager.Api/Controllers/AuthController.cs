using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.DTOs.Auth;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

/// <summary>
/// Gerenciamento de autenticação e registro de usuários
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "🔐 Autenticação")]
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
    /// Registra um novo usuário no sistema
    /// </summary>
    /// <param name="request">Dados de registro do usuário</param>
    /// <returns>Token JWT e informações do usuário</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/v1/auth/register
    ///     {
    ///         "username": "herouser",
    ///         "email": "hero@eldoria.com",
    ///         "password": "senha123"
    ///     }
    ///     
    /// Regras de validação:
    /// * Username: mínimo 3 caracteres, máximo 50
    /// * Email: formato válido
    /// * Password: mínimo 6 caracteres
    /// 
    /// </remarks>
    /// <response code="200">Usuário registrado com sucesso. Retorna token JWT.</response>
    /// <response code="400">Dados inválidos ou usuário/email já existe</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Tentativa de registro para usuário: {Username}", request.Username);
        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }
    
    /// <summary>
    /// Autentica um usuário existente e retorna token JWT
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Token JWT válido por 24 horas</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/v1/auth/login
    ///     {
    ///         "username": "herouser",
    ///         "password": "senha123"
    ///     }
    ///     
    /// O token retornado deve ser usado no header Authorization:
    /// 
    ///     Authorization: Bearer {seu_token_aqui}
    ///     
    /// </remarks>
    /// <response code="200">Login bem-sucedido. Retorna token JWT.</response>
    /// <response code="400">Credenciais inválidas</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Tentativa de login para usuário: {Username}", request.Username);
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }
}

