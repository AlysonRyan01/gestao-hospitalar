using gestao_hospitalar.Domain.Consultas.Aggregates;
using gestao_hospitalar.Domain.Consultas.Enums;
using gestao_hospitalar.Domain.Shared.Contracts;
using gestao_hospitalar.Domain.Users.Aggregates;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Domain.Pacientes.Aggregates;

public class Paciente : AggregateRoot
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    private readonly List<Consulta> _consultas = new();
    public IReadOnlyCollection<Consulta> Consultas => _consultas;

    protected Paciente() { }

    private Paciente(Guid userId)
    {
        UserId = userId;
    }

    public static Result<Paciente> Criar(Guid userId)
    {
        var paciente = new Paciente(userId);
        return Result<Paciente>.Success(paciente);
    }

    public Result AdicionarConsulta(Consulta consulta)
    {
        _consultas.Add(consulta);
        return Result.Success();
    }

    public List<Consulta> VerConsultasSolicitadas()
        => Consultas.Where(c => c.Status == EStatusConsulta.AgendamentoSolicitado).ToList();

    public List<Consulta> VerConsultasMarcadas()
        => Consultas.Where(c => c.Status == EStatusConsulta.AgendamentoMarcado).ToList();

    public List<Consulta> VerConsultasConcluidas()
        => Consultas.Where(c => c.Status == EStatusConsulta.ConsultaConcluida).ToList();

    public List<Consulta> VerConsultasCanceladas()
        => Consultas.Where(c => c.Status == EStatusConsulta.ConsultaCancelada).ToList();
}