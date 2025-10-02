using FluentValidation;
using RpgQuestManager.Api.DTOs.Auth;

namespace RpgQuestManager.Api.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username é obrigatório")
            .MinimumLength(3).WithMessage("Username deve ter no mínimo 3 caracteres")
            .MaximumLength(50).WithMessage("Username deve ter no máximo 50 caracteres");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(100).WithMessage("Email deve ter no máximo 100 caracteres");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password é obrigatório")
            .MinimumLength(6).WithMessage("Password deve ter no mínimo 6 caracteres");
    }
}

