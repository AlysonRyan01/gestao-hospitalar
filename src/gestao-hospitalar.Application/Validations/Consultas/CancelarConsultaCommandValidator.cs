using FluentValidation;
using gestao_hospitalar.Application.Commands.Consultas;

namespace gestao_hospitalar.Application.Validations.Consultas;

public class CancelarConsultaCommandValidator : AbstractValidator<CancelarConsultaCommand>
{
    public CancelarConsultaCommandValidator()
    {
        RuleFor(x => x.MotivoCancelamento)
            .NotEmpty()
            .WithMessage("É obrigatório informar o motivo do cancelamento.");
    }
}