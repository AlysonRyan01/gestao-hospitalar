namespace gestao_hospitalar.Domain.Shared.Contracts;

public abstract class AggregateRoot
{
    public Guid Id { get; protected set; }
}