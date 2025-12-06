using FluentValidation;
using gestao_hospitalar.Application.Commands.Consultas;

namespace gestao_hospitalar.Application.Validations.Consultas;

public class SolicitarAgendamentoCommandValidator : AbstractValidator<SolicitarAgendamentoCommand>
{
    public SolicitarAgendamentoCommandValidator()
    {
        RuleFor(x => x.Sobre)
            .NotEmpty()
            .WithMessage("Informar o problema é obrigatório.");

        RuleFor(x => x.MarcarPara)
            .Must(DataValida)
            .WithMessage("A data e hora da consulta devem ser durante o expediente (09:00-12:00, 13:00-18:00) e em dias úteis.");
    }

    private bool DataValida(DateTime data)
    {
        if (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday)
            return false;

        var hora = data.TimeOfDay;
        var inicioManha = TimeSpan.FromHours(9);
        var inicioAlmoco = TimeSpan.FromHours(12);
        var fimAlmoco = TimeSpan.FromHours(13);
        var fimTarde = TimeSpan.FromHours(18);

        if (hora < inicioManha) return false;
        if (hora >= inicioAlmoco && hora < fimAlmoco) return false;
        if (hora >= fimTarde) return false;

        return true;
    }
}