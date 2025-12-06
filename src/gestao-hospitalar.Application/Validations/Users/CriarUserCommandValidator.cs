using System.Text.RegularExpressions;
using FluentValidation;
using gestao_hospitalar.Application.Commands.Users;

namespace gestao_hospitalar.Application.Validations.Users;

public class CriarUserCommandValidator : AbstractValidator<CriarUserCommand>
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private static readonly Regex PhoneRegex =
        new(@"^\+?(\d{2})?\s?\(?\d{2}\)?\s?\d{4,5}-?\d{4}$", RegexOptions.Compiled);

    public CriarUserCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .Matches(EmailRegex).WithMessage("Email inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.")
            .MaximumLength(50).WithMessage("A senha deve ter no máximo 50 caracteres.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("O telefone é obrigatório.")
            .Matches(PhoneRegex).WithMessage("Telefone inválido.");
    }
}