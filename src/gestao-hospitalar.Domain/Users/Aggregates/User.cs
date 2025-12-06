using System.Text.RegularExpressions;
using gestao_hospitalar.Domain.Shared.Contracts;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Domain.Users.Aggregates;

public class User : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Password { get; private set; } = null!;
    public string Phone { get; private set; } = null!;

    protected User() { }

    private User(string name, string email, string password, string phone)
    {
        Name = name;
        Email = email;
        Password = password;
        Phone = phone;
    }

    public static Result<User> Criar(string name, string email, string password, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<User>.Failure("Nome inválido");

        if (!EmailRegex.IsMatch(email))
            return Result<User>.Failure("Email inválido");

        if (string.IsNullOrWhiteSpace(password))
            return Result<User>.Failure("Senha obrigatória");

        if (!PhoneRegex.IsMatch(phone))
            return Result<User>.Failure("Telefone inválido");

        var user = new User(name, email, password, phone);
        return Result<User>.Success(user);
    }

    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private static readonly Regex PhoneRegex =
        new(@"^\+?(\d{2})?\s?\(?\d{2}\)?\s?\d{4,5}-?\d{4}$", RegexOptions.Compiled);
}