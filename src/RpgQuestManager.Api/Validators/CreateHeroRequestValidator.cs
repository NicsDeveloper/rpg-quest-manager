using FluentValidation;
using RpgQuestManager.Api.DTOs.Heroes;

namespace RpgQuestManager.Api.Validators;

public class CreateHeroRequestValidator : AbstractValidator<CreateHeroRequest>
{
    public CreateHeroRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");
        
        RuleFor(x => x.Class)
            .NotEmpty().WithMessage("Classe é obrigatória")
            .Must(c => new[] { "Guerreiro", "Mago", "Arqueiro", "Paladino", "Ladino" }.Contains(c))
            .WithMessage("Classe inválida. Opções: Guerreiro, Mago, Arqueiro, Paladino, Ladino");
        
        RuleFor(x => x.Strength)
            .GreaterThanOrEqualTo(1).WithMessage("Força deve ser maior ou igual a 1")
            .LessThanOrEqualTo(100).WithMessage("Força deve ser menor ou igual a 100");
        
        RuleFor(x => x.Intelligence)
            .GreaterThanOrEqualTo(1).WithMessage("Inteligência deve ser maior ou igual a 1")
            .LessThanOrEqualTo(100).WithMessage("Inteligência deve ser menor ou igual a 100");
        
        RuleFor(x => x.Dexterity)
            .GreaterThanOrEqualTo(1).WithMessage("Destreza deve ser maior ou igual a 1")
            .LessThanOrEqualTo(100).WithMessage("Destreza deve ser menor ou igual a 100");
    }
}

