using gestao_hospitalar.Domain.Consultas.Aggregates;
using gestao_hospitalar.Domain.Medicos.Aggregates;
using gestao_hospitalar.Domain.Pacientes.Aggregates;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Tests.Medicos.Aggregates;

[TestClass]
public class MedicoTestes
{
    private Medico _medico = null!;
    private Paciente _paciente = null!;

    [TestInitialize]
    public void Setup()
    {
        var userResult = Domain.Users.Aggregates.User.Criar("João Silva", "joao@email.com", "senha123", "+5511999999999");
        var pacienteResult = Paciente.Criar(userResult.Data!);
        _paciente = pacienteResult.Data!;

        var medicoResult = Medico.Criar("Dr. Carlos", "+5511988888888", "Cardiologia");
        Assert.AreEqual(EStatus.Success, medicoResult.Status);
        _medico = medicoResult.Data!;
    }

    [TestMethod]
    public void DeveCriarMedicoComSucesso()
    {
        var result = Medico.Criar("Dra. Ana", "+5511977777777", "Neurologia");
        Assert.AreEqual(EStatus.Success, result.Status);
        Assert.AreEqual("Dra. Ana", result.Data!.Nome);
        Assert.AreEqual("+5511977777777", result.Data!.Telefone);
        Assert.AreEqual("Neurologia", result.Data!.Especialidade);
    }

    [TestMethod]
    public void NaoDeveCriarMedicoComDadosInvalidos()
    {
        var result1 = Medico.Criar("", "+5511977777777", "Neurologia");
        Assert.AreEqual(EStatus.Failure, result1.Status);

        var result2 = Medico.Criar("Dra. Ana", "telefone_invalido", "Neurologia");
        Assert.AreEqual(EStatus.Failure, result2.Status);

        var result3 = Medico.Criar("Dra. Ana", "+5511977777777", "");
        Assert.AreEqual(EStatus.Failure, result3.Status);
    }

    [TestMethod]
    public void DeveAtualizarMedicoComSucesso()
    {
        var result = _medico.Atualizar("Dr. Carlos Atualizado", "+5511988888888", "Cardiologia");
        Assert.AreEqual(EStatus.Success, result.Status);
        Assert.AreEqual("Dr. Carlos Atualizado", _medico.Nome);
    }

    [TestMethod]
    public void NaoDeveAtualizarMedicoComDadosInvalidos()
    {
        var result = _medico.Atualizar("", "+5511988888888", "Cardiologia");
        Assert.AreEqual(EStatus.Failure, result.Status);
    }

    [TestMethod]
    public void DeveValidarHorarioDisponivelComSucesso()
    {
        var dataConsulta = new DateTime(2025, 12, 8, 10, 0, 0);
        var consulta = Consulta.SolicitarAgendamento(_paciente, "Dor de cabeça", dataConsulta).Data!;
        var result = _medico.ValidarHorarioDisponivel(consulta, 30);

        Assert.AreEqual(EStatus.Success, result.Status);
    }

    [TestMethod]
    public void NaoDeveValidarHorarioQuandoConflitoExistir()
    {
        var dataConsulta1 = new DateTime(2025, 12, 8, 10, 0, 0);
        var consulta1 = Consulta.SolicitarAgendamento(_paciente, "Dor de cabeça", dataConsulta1).Data!;
        consulta1.AgendarConsulta(_medico, 60);

        var dataConsulta2 = new DateTime(2025, 12, 8, 10, 30, 0);
        var consulta2 = Consulta.SolicitarAgendamento(_paciente, "Febre", dataConsulta2).Data!;

        var result = _medico.ValidarHorarioDisponivel(consulta2, 30);
        Assert.AreEqual(EStatus.Failure, result.Status);
    }

    [TestMethod]
    public void DeveAdicionarConsultaAoMedico()
    {
        var dataConsulta = new DateTime(2025, 12, 8, 11, 0, 0);
        var consulta = Consulta.SolicitarAgendamento(_paciente, "Consulta teste", dataConsulta).Data!;

        var result = _medico.AdicionarConsulta(consulta);
        Assert.AreEqual(EStatus.Success, result.Status);
        Assert.HasCount(1, _medico.Consultas);
    }
}