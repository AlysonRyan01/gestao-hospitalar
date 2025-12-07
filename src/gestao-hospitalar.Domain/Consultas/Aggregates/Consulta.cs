using gestao_hospitalar.Domain.Consultas.Enums;
using gestao_hospitalar.Domain.Medicos.Aggregates;
using gestao_hospitalar.Domain.Pacientes.Aggregates;
using gestao_hospitalar.Domain.Shared.Contracts;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Domain.Consultas.Aggregates;

public class Consulta : AggregateRoot
{
    public string Sobre { get; private set; } = null!;
    public DateTime MarcadoPara { get; private set; }
    public DateTime? FinalConsultaPara { get; private set; }
    public string? MotivoCancelamento { get; private set; }
    public EStatusConsulta Status { get; private set; }

    public Guid PacienteId { get; private set; }
    public Paciente Paciente { get; private set; } = null!;

    public Guid? MedicoId { get; set; }
    public Medico? Medico { get; private set; }

    protected Consulta() { }

    private Consulta(Guid pacienteId, string sobre, DateTime marcadoPara)
    {
        PacienteId = pacienteId;
        Sobre = sobre;
        MarcadoPara = marcadoPara;
        Status = EStatusConsulta.AgendamentoSolicitado;
    }

    public static Result<Consulta> SolicitarAgendamento(Guid pacienteId, string sobre, DateTime marcarPara)
    {
        if (string.IsNullOrEmpty(sobre))
            return Result<Consulta>.Failure("Informar seu problema é obrigatório");
        
        var diaHoraValida = ValidarHoraConsulta(marcarPara);
        if (diaHoraValida.Status == EStatus.Failure)
            return Result<Consulta>.Failure(diaHoraValida.Mensagem!);

        var consulta = new Consulta(pacienteId, sobre, marcarPara);
        
        return Result<Consulta>.Success(consulta);
    }

    public Result AgendarConsulta(Guid medicoId, int duracaoDaConsultaEmMinutos)
    {
        if (Status == EStatusConsulta.ConsultaCancelada)
            return Result.Failure("Não é possível agendar uma consulta cancelada.");
        
        FinalConsultaPara = MarcadoPara.AddMinutes(duracaoDaConsultaEmMinutos);
        Status = EStatusConsulta.AgendamentoMarcado;
        
        return Result.Success();
    }

    public Result ConcluirConsulta()
    {
        if (Status != EStatusConsulta.AgendamentoMarcado)
            return Result.Failure("A consulta precisa estar marcada para ser concluída.");
        
        Status = EStatusConsulta.ConsultaConcluida;
        
        return Result.Success();
    }

    public Result CancelarConsulta(string motivoCancelamento)
    {
        if (string.IsNullOrEmpty(motivoCancelamento))
            return Result.Failure("É obrigatório informar o motivo do cancelamento");
        
        Status = EStatusConsulta.ConsultaCancelada;
        MotivoCancelamento = motivoCancelamento;
        
        return Result.Success();
    }

    private static Result ValidarHoraConsulta(DateTime marcadoPara)
    {
        if (marcadoPara.DayOfWeek == DayOfWeek.Saturday ||
            marcadoPara.DayOfWeek == DayOfWeek.Sunday)
            return Result.Failure("Consultas não podem ser marcadas aos sábados ou domingos.");
        
        var hora = marcadoPara.TimeOfDay;

        var inicioManha = TimeSpan.FromHours(9);

        var inicioAlmoco = TimeSpan.FromHours(12);
        var fimAlmoco = TimeSpan.FromHours(13);
        
        var fimTarde = TimeSpan.FromHours(18);
        
        if (hora < inicioManha)
            return Result.Failure("Consultas só podem ser marcadas a partir das 09:00.");
        
        if (hora >= inicioAlmoco && hora < fimAlmoco)
            return Result.Failure("Não é possível marcar consulta no horário de almoço (12:00 às 13:00).");
        
        if (hora >= fimTarde)
            return Result.Failure("Consultas só podem ser marcadas até às 18:00.");

        return Result.Success();
    }
}