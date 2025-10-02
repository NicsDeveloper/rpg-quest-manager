using FluentValidation;
using RpgQuestManager.Api.DTOs.Quests;

namespace RpgQuestManager.Api.Validators;

public class CreateQuestRequestValidator : AbstractValidator<CreateQuestRequest>
{
    public CreateQuestRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");
        
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");
        
        RuleFor(x => x.Difficulty)
            .NotEmpty().WithMessage("Dificuldade é obrigatória")
            .Must(d => new[] { "Fácil", "Médio", "Difícil", "Épico" }.Contains(d))
            .WithMessage("Dificuldade inválida. Opções: Fácil, Médio, Difícil, Épico");
        
        RuleFor(x => x.ExperienceReward)
            .GreaterThanOrEqualTo(0).WithMessage("Experiência deve ser maior ou igual a 0");
        
        RuleFor(x => x.GoldReward)
            .GreaterThanOrEqualTo(0).WithMessage("Ouro deve ser maior ou igual a 0");
    }
}

