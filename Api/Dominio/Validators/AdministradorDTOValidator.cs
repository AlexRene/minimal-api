using FluentValidation;
using MinimalApi.Dominio.DTOs;

namespace MinimalApi.Dominio.Validators;

public class AdministradorDTOValidator : AbstractValidator<AdministradorDTO>
{
    public AdministradorDTOValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ter um formato válido")
            .MaximumLength(255).WithMessage("Email não pode ter mais de 255 caracteres");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres")
            .MaximumLength(50).WithMessage("Senha não pode ter mais de 50 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$")
            .WithMessage("Senha deve conter pelo menos uma letra maiúscula, uma minúscula e um número");

        RuleFor(x => x.Perfil)
            .NotNull().WithMessage("Perfil é obrigatório")
            .IsInEnum().WithMessage("Perfil deve ser um valor válido");
    }
}
