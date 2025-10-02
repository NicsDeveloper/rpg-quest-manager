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
            .Must(c => new[] { 
                "Guerreiro", "Mago", "Arqueiro", "Paladino", "Ladrão", 
                "Clérigo", "Bárbaro", "Bruxo", "Druida", "Monge" 
            }.Contains(c))
            .WithMessage("Classe inválida. Opções: Guerreiro, Mago, Arqueiro, Paladino, Ladrão, Clérigo, Bárbaro, Bruxo, Druida, Monge");
    }
}

