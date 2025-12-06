using System.Text.RegularExpressions;
using gestao_hospitalar.Domain.Consultas.Aggregates;
using gestao_hospitalar.Domain.Consultas.Enums;
using gestao_hospitalar.Domain.Shared.Contracts;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Domain.Medicos.Aggregates;

public class Medico : AggregateRoot
{
    public string Nome { get; private set; } = null!;
    public string Telefone { get; private set; } = null!;
    public string Especialidade { get; private set; } = null!;
    
    private readonly List<Consulta> _consultas = new();
    public IReadOnlyCollection<Consulta> Consultas => _consultas;

    protected Medico() { }

    private Medico(string nome, string telefone, string especialidade)
    {
        Nome = nome;
        Telefone = telefone;
        Especialidade = especialidade;
    }

    public static Result<Medico> Criar(string nome, string telefone, string especialidade)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result<Medico>.Failure("Nome inválido");

        if (!TelefoneRegex.IsMatch(telefone))
            return Result<Medico>.Failure("Telefone inválido");

        if (string.IsNullOrWhiteSpace(especialidade))
            return Result<Medico>.Failure("Especialidade obrigatória");

        var medico = new Medico(nome, telefone, especialidade);
        
        return Result<Medico>.Success(medico);
    }

    public Result<Medico> Atualizar(string nome, string telefone, string especialidade)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result<Medico>.Failure("Nome inválido");

        if (!TelefoneRegex.IsMatch(telefone))
            return Result<Medico>.Failure("Telefone inválido");

        if (string.IsNullOrWhiteSpace(especialidade))
            return Result<Medico>.Failure("Especialidade obrigatória");
        
        Nome = nome;
        Telefone = telefone;
        Especialidade = especialidade;
        
        return Result<Medico>.Success(this);
    }

    internal Result ValidarHorarioDisponivel(Consulta consulta, int  duracaoDaConsultaEmMinutos)
    {
        if (duracaoDaConsultaEmMinutos < 5 || duracaoDaConsultaEmMinutos > 180)
            return Result.Failure("A duração da consulta deve ser maior que 5 minutos e menor que 3 horas");
        
        var inicio = consulta.MarcadoPara;
        var fim = consulta.MarcadoPara.AddMinutes(duracaoDaConsultaEmMinutos);

        var conflito = Consultas.FirstOrDefault(c =>
            c.Status == EStatusConsulta.AgendamentoMarcado &&
            c.MarcadoPara < fim &&
            c.FinalConsultaPara > inicio
        );

        if (conflito != null)
            return Result.Failure("Horário indisponível para marcar a consulta.");

        return Result.Success();
    }
    
    internal Result AdicionarConsulta(Consulta consulta)
    {
        _consultas.Add(consulta);
        return Result.Success();
    }

    public List<Consulta> VerConsultasMarcadas() 
        => Consultas
            .Where(c => c.Status == EStatusConsulta.AgendamentoMarcado)
            .ToList();
    
    public List<Consulta> VerConsultasConcluidas() 
        => Consultas
            .Where(c => c.Status == EStatusConsulta.ConsultaConcluida)
            .ToList();
    
    public List<Consulta> VerConsultasCanceladas() 
        => Consultas
            .Where(c => c.Status == EStatusConsulta.ConsultaCancelada)
            .ToList();

    private static readonly Regex TelefoneRegex =
        new(@"^\+?(\d{2})?\s?\(?\d{2}\)?\s?\d{4,5}-?\d{4}$", RegexOptions.Compiled);
}