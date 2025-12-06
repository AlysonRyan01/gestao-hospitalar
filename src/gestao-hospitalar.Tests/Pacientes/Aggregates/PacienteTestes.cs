using gestao_hospitalar.Domain.Consultas.Aggregates;
using gestao_hospitalar.Domain.Medicos.Aggregates;
using gestao_hospitalar.Domain.Pacientes.Aggregates;
using gestao_hospitalar.Domain.Users.Aggregates;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Tests.Pacientes.Aggregates;

[TestClass]
public class PacienteTestes
{
    private User _user = null!;
    private Paciente _paciente = null!;

    [TestInitialize]
    public void Setup()
    {
        var userResult = User.Criar("João Silva", "joao@email.com", "senha123", "+5511999999999");
        Assert.AreEqual(EStatus.Success, userResult.Status);
        _user = userResult.Data!;

        var pacienteResult = Paciente.Criar(_user);
        Assert.AreEqual(EStatus.Success, pacienteResult.Status);
        _paciente = pacienteResult.Data!;
    }

    [TestMethod]
    public void DeveCriarPacienteComSucesso()
    {
        Assert.AreEqual(_user, _paciente.User);
        Assert.AreEqual(_user.Id, _paciente.UserId);
    }

    [TestMethod]
    public void DeveAdicionarConsultaAoPaciente()
    {
        var dataConsulta = new DateTime(2025, 12, 8, 10, 0, 0);
        Consulta.SolicitarAgendamento(_paciente, "Dor de cabeça", dataConsulta);
        
        Assert.HasCount(1, _paciente.Consultas);
    }

    [TestMethod]
    public void DeveRetornarConsultasPorStatus()
    {
        var dataConsulta1 = new DateTime(2025, 12, 8, 10, 0, 0);
        var dataConsulta2 = new DateTime(2025, 12, 8, 11, 0, 0);

        var consulta1 = Consulta.SolicitarAgendamento(_paciente, "Consulta 1", dataConsulta1).Data!;
        var consulta2 = Consulta.SolicitarAgendamento(_paciente, "Consulta 2", dataConsulta2).Data!;

        consulta1.AgendarConsulta(Medico.Criar("Dr. Carlos", "+5511988888888", "Cardiologia").Data!, 30);
        consulta2.CancelarConsulta("Paciente não pode comparecer");

        var solicitadas = _paciente.VerConsultasSolicitadas();
        var marcadas = _paciente.VerConsultasMarcadas();
        var canceladas = _paciente.VerConsultasCanceladas();
        var concluidas = _paciente.VerConsultasConcluidas();

        Assert.HasCount(0, solicitadas);
        Assert.HasCount(1, marcadas);
        Assert.HasCount(1, canceladas);
        Assert.HasCount(0, concluidas);
    }
}