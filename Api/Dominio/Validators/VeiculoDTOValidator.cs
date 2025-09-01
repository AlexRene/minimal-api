using FluentValidation;
using MinimalApi.Dominio.DTOs;

namespace MinimalApi.Dominio.Validators;

public class VeiculoDTOValidator : AbstractValidator<VeiculoDTO>
{
    public VeiculoDTOValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MinimumLength(2).WithMessage("Nome deve ter pelo menos 2 caracteres")
            .MaximumLength(150).WithMessage("Nome não pode ter mais de 150 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s\-\.]+$").WithMessage("Nome deve conter apenas letras, espaços, hífens e pontos");

        RuleFor(x => x.Marca)
            .NotEmpty().WithMessage("Marca é obrigatória")
            .MinimumLength(2).WithMessage("Marca deve ter pelo menos 2 caracteres")
            .MaximumLength(100).WithMessage("Marca não pode ter mais de 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s\-\.]+$").WithMessage("Marca deve conter apenas letras, espaços, hífens e pontos");

        RuleFor(x => x.Ano)
            .NotEmpty().WithMessage("Ano é obrigatório")
            .InclusiveBetween(1950, DateTime.Now.Year + 1)
            .WithMessage($"Ano deve estar entre 1950 e {DateTime.Now.Year + 1}");
    }
}
