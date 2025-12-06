using gestao_hospitalar.Application.Dtos.Users;
using gestao_hospitalar.Domain.Users.Aggregates;

namespace gestao_hospitalar.Application.Mappers;

public static class UserMapper
{
    public static UserDto EntityToDto(this User user)
        => new UserDto(user.Id, user.Name, user.Email, user.Phone);
}