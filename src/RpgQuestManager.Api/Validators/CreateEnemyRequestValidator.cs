using FluentValidation;
using RpgQuestManager.Api.DTOs.Enemies;

namespace RpgQuestManager.Api.Validators;

public class CreateEnemyRequestValidator : AbstractValidator<CreateEnemyRequest>
{
    public CreateEnemyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");
        
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Tipo é obrigatório")
            .MaximumLength(50).WithMessage("Tipo deve ter no máximo 50 caracteres");
        
        RuleFor(x => x.Power)
            .GreaterThanOrEqualTo(1).WithMessage("Poder deve ser maior ou igual a 1");
        
        RuleFor(x => x.Health)
            .GreaterThanOrEqualTo(1).WithMessage("Vida deve ser maior ou igual a 1");
    }
}

