using System.Text.RegularExpressions;
using FluentValidation;
using gestao_hospitalar.Application.Commands.Medicos;

namespace gestao_hospitalar.Application.Validations.Medicos;

public class AtualizarMedicoCommandValidator : AbstractValidator<AtualizarMedicoCommand>
{
    private static readonly Regex TelefoneRegex =
        new(@"^\+?(\d{2})?\s?\(?\d{2}\)?\s?\d{4,5}-?\d{4}$", RegexOptions.Compiled);

    public AtualizarMedicoCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome do médico é obrigatório.");

        RuleFor(x => x.Telefone)
            .NotEmpty().WithMessage("Telefone do médico é obrigatório.")
            .Matches(TelefoneRegex).WithMessage("Telefone inválido.");

        RuleFor(x => x.Especialidade)
            .NotEmpty().WithMessage("Especialidade do médico é obrigatória.");
    }
}