using gestao_hospitalar.Domain.Consultas.Aggregates;
using gestao_hospitalar.Domain.Consultas.Enums;
using gestao_hospitalar.Domain.Medicos.Aggregates;
using gestao_hospitalar.Domain.Pacientes.Aggregates;
using gestao_hospitalar.Domain.Users.Aggregates;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Tests.Consultas.Aggregates;

[TestClass]
public class ConsultaTestes
{
    private Paciente _paciente = null!;
    private Medico _medico = null!;

    [TestInitialize]
    public void Setup()
    {
        var userResult = User.Criar("João Silva", "joao@email.com", "senha123", "+5511999999999");
        Assert.AreEqual(EStatus.Success, userResult.Status);

        var pacienteResult = Paciente.Criar(userResult.Data!);
        Assert.AreEqual(EStatus.Success, pacienteResult.Status);
        _paciente = pacienteResult.Data!;
        
        var medicoResult = Medico.Criar("Dr. Carlos", "+5511988888888", "Cardiologia");
        Assert.AreEqual(EStatus.Success, medicoResult.Status);
        _medico = medicoResult.Data!;
    }

    [TestMethod]
    public void DeveCriarConsultaComSucesso()
    {
        var dataConsulta = new DateTime(2025, 12, 8, 10, 0, 0);
        var resultado = Consulta.SolicitarAgendamento(_paciente, "Dor no peito", dataConsulta);

        Assert.AreEqual(EStatus.Success, resultado.Status);
        Assert.AreEqual(EStatusConsulta.AgendamentoSolicitado, resultado.Data!.Status);
        Assert.AreEqual("Dor no peito", resultado.Data!.Sobre);
        Assert.AreEqual(_paciente, resultado.Data.Paciente);
    }

    [TestMethod]
    public void NaoDeveCriarConsultaComHorarioInvalido()
    {
        var dataConsulta = new DateTime(2025, 12, 7, 10, 0, 0);
        var resultado = Consulta.SolicitarAgendamento(_paciente, "Dor no peito", dataConsulta);

        Assert.AreEqual(EStatus.Failure, resultado.Status);
    }

    [TestMethod]
    public void DeveAgendarConsultaComSucesso()
    {
        var dataConsulta = new DateTime(2025, 12, 8, 10, 0, 0); 
        var consulta = Consulta.SolicitarAgendamento(_paciente, "Dor no peito", dataConsulta).Data;

        var resultadoAgendar = consulta!.AgendarConsulta(_medico, 30);
        Assert.AreEqual(EStatus.Success, resultadoAgendar.Status);
        Assert.AreEqual(EStatusConsulta.AgendamentoMarcado, consulta.Status);
        Assert.AreEqual(_medico, consulta.Medico);
        Assert.AreEqual(dataConsulta.AddMinutes(30), consulta.FinalConsultaPara);
    }

    [TestMethod]
    public void NaoDeveAgendarConsultaCancelada()
    {
        var dataConsulta = new DateTime(2025, 12, 8, 10, 0, 0);
        var consulta = Consulta.SolicitarAgendamento(_paciente, "Dor no peito", dataConsulta).Data;
        consulta!.CancelarConsulta("Paciente não pode comparecer");

        var resultadoAgendar = consulta.AgendarConsulta(_medico, 30);
        Assert.AreEqual(EStatus.Failure, resultadoAgendar.Status);
    }

    [TestMethod]
    public void DeveConcluirConsulta()
    {
        var dataConsulta = new DateTime(2025, 12, 8, 10, 0, 0);
        var consulta = Consulta.SolicitarAgendamento(_paciente, "Dor no peito", dataConsulta).Data;
        consulta!.AgendarConsulta(_medico, 30);

        var resultadoConcluir = consulta.ConcluirConsulta();
        Assert.AreEqual(EStatus.Success, resultadoConcluir.Status);
        Assert.AreEqual(EStatusConsulta.ConsultaConcluida, consulta.Status);
    }

    [TestMethod]
    public void NaoDeveConcluirConsultaNaoMarcada()
    {
        var dataConsulta = new DateTime(2025, 12, 8, 10, 0, 0);
        var consulta = Consulta.SolicitarAgendamento(_paciente, "Dor no peito", dataConsulta).Data;

        var resultadoConcluir = consulta!.ConcluirConsulta();
        Assert.AreEqual(EStatus.Failure, resultadoConcluir.Status);
    }

    [TestMethod]
    public void DeveCancelarConsulta()
    {
        var dataConsulta = new DateTime(2025, 12, 8, 10, 0, 0);
        var consulta = Consulta.SolicitarAgendamento(_paciente, "Dor no peito", dataConsulta).Data;

        var resultadoCancelar = consulta!.CancelarConsulta("Paciente não pode comparecer");
        Assert.AreEqual(EStatus.Success, resultadoCancelar.Status);
        Assert.AreEqual(EStatusConsulta.ConsultaCancelada, consulta.Status);
        Assert.AreEqual("Paciente não pode comparecer", consulta.MotivoCancelamento);
    }

    [TestMethod]
    public void NaoDeveCancelarConsultaSemMotivo()
    {
        var dataConsulta = new DateTime(2025, 12, 8, 10, 0, 0);
        var consulta = Consulta.SolicitarAgendamento(_paciente, "Dor no peito", dataConsulta).Data;

        var resultadoCancelar = consulta!.CancelarConsulta("");
        Assert.AreEqual(EStatus.Failure, resultadoCancelar.Status);
    }
}