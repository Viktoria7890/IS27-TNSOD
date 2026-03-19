namespace UserService.Data.DbModels;


public sealed record UserDbModel(
    int Id,
    string Login,
    string Password,
    string Name,
    string Surname,
    int Age
);