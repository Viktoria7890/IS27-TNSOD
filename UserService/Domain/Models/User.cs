namespace UserService.Domain.Models;

public sealed record User(
    int Id,
    string Login,
    string Password,
    string Name,
    string Surname,
    int Age
);