using gestao_hospitalar.Domain.Users.Aggregates;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Tests.Users.Aggregates;

[TestClass]
public class UserTestes
{
    [TestMethod]
    public void DeveCriarUsuarioComSucesso()
    {
        var resultado = User.Criar("João Silva", "joao@email.com", "senha123", "+5511999999999");

        Assert.AreEqual(EStatus.Success, resultado.Status);
        Assert.IsNotNull(resultado.Data);
        Assert.AreEqual("João Silva", resultado.Data!.Name);
        Assert.AreEqual("joao@email.com", resultado.Data.Email);
        Assert.AreEqual("senha123", resultado.Data.Password);
        Assert.AreEqual("+5511999999999", resultado.Data.Phone);
    }

    [TestMethod]
    public void NaoDeveCriarUsuarioSemNome()
    {
        var resultado = User.Criar("", "joao@email.com", "senha123", "+5511999999999");

        Assert.AreEqual(EStatus.Failure, resultado.Status);
        Assert.AreEqual("Nome inválido", resultado.Mensagem);
    }

    [TestMethod]
    public void NaoDeveCriarUsuarioComEmailInvalido()
    {
        var resultado = User.Criar("João Silva", "email-invalido", "senha123", "+5511999999999");

        Assert.AreEqual(EStatus.Failure, resultado.Status);
        Assert.AreEqual("Email inválido", resultado.Mensagem);
    }

    [TestMethod]
    public void NaoDeveCriarUsuarioSemSenha()
    {
        var resultado = User.Criar("João Silva", "joao@email.com", "", "+5511999999999");

        Assert.AreEqual(EStatus.Failure, resultado.Status);
        Assert.AreEqual("Senha obrigatória", resultado.Mensagem);
    }

    [TestMethod]
    public void NaoDeveCriarUsuarioComTelefoneInvalido()
    {
        var resultado = User.Criar("João Silva", "joao@email.com", "senha123", "telefone-invalido");

        Assert.AreEqual(EStatus.Failure, resultado.Status);
        Assert.AreEqual("Telefone inválido", resultado.Mensagem);
    }
}