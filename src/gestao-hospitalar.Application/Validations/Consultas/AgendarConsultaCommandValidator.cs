using FluentValidation;
using gestao_hospitalar.Application.Commands.Consultas;

namespace gestao_hospitalar.Application.Validations.Consultas;

public class AgendarConsultaCommandValidator : AbstractValidator<AgendarConsultaCommand>
{
    public AgendarConsultaCommandValidator()
    {
        RuleFor(x => x.DuracaoEmMinutos)
            .GreaterThanOrEqualTo(5)
            .WithMessage("A duração da consulta deve ser no mínimo 5 minutos.")
            .LessThanOrEqualTo(180)
            .WithMessage("A duração da consulta não pode exceder 180 minutos.");
    }
}