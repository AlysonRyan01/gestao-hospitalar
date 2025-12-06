namespace gestao_hospitalar.Shared;

public class Result<T>
{
    public T? Data { get; private set; }
    public EStatus Status { get; private set; }
    public string? Mensagem { get; private set; }

    private Result(T data)
    {
        Data = data;
        Status = EStatus.Success;
    }

    private Result(string mensagem)
    {
        Mensagem = mensagem;
        Status = EStatus.Failure;
    }

    public static Result<T> Success(T data)
        => new Result<T>(data);

    public static Result<T> Failure(string mensagem)
        => new Result<T>(mensagem);
}

public class Result
{
    public EStatus Status { get; private set; }
    public string? Mensagem { get; private set; }

    private Result()
    {
        Status = EStatus.Success;
    }

    private Result(string mensagem)
    {
        Mensagem = mensagem;
        Status = EStatus.Failure;
    }

    public static Result Success()
        => new Result();

    public static Result Failure(string mensagem)
        => new Result(mensagem);
}

public enum EStatus
{
    Success,
    Failure
}