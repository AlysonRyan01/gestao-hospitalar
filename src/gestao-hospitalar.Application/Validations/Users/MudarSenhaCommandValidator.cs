using FluentValidation;
using gestao_hospitalar.Application.Commands.Users;

namespace gestao_hospitalar.Application.Validations.Users;

public class MudarSenhaCommandValidator : AbstractValidator<MudarSenhaCommand>
{
    public MudarSenhaCommandValidator()
    {
        RuleFor(x => x.SenhaAtual)
            .NotEmpty().WithMessage("A senha atual é obrigatória.");

        RuleFor(x => x.NovaSenha)
            .NotEmpty().WithMessage("A nova senha é obrigatória.")
            .MinimumLength(6).WithMessage("A nova senha deve ter pelo menos 6 caracteres.")
            .Matches(@"[A-Z]").WithMessage("A nova senha deve conter pelo menos uma letra maiúscula.")
            .Matches(@"[a-z]").WithMessage("A nova senha deve conter pelo menos uma letra minúscula.")
            .Matches(@"[0-9]").WithMessage("A nova senha deve conter pelo menos um número.")
            .Matches(@"[\W]").WithMessage("A nova senha deve conter pelo menos um caractere especial.");
    }
}