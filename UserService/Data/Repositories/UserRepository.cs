using Dapper;
using Npgsql;
using UserService.Data.DbModels;
using UserService.Domain.Models;
using UserService.Mappings;

namespace UserService.Data.Repositories;

public sealed class UserRepository(NpgsqlDataSource dataSource) : IUserRepository
{
    public async Task<bool> IsLoginExistsAsync(string login, CancellationToken ct)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct);
        var cmd = new CommandDefinition(
            "select app.fn_is_login_exists(@p_login);",
            new { p_login = login },
            cancellationToken: ct
        );
        return await conn.ExecuteScalarAsync<bool>(cmd);
    }

    public async Task<User> CreateUserAsync(User user, CancellationToken ct)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct);
        var cmd = new CommandDefinition(
            "select * from app.fn_create_user(@p_login, @p_password, @p_name, @p_surname, @p_age);",
            new
            {
                p_login = user.Login,
                p_password = user.Password,
                p_name = user.Name,
                p_surname = user.Surname,
                p_age = user.Age
            },
            cancellationToken: ct
        );
        var db = await conn.QuerySingleAsync<UserDbModel>(cmd);
        return UserMapper.ToDomain(db);
    }

    public async Task<User?> GetUserByIdAsync(int id, CancellationToken ct)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct);
        var cmd = new CommandDefinition(
            "select * from app.fn_get_user_by_id(@p_id);",
            new { p_id = id },
            cancellationToken: ct
        );
        var db = await conn.QuerySingleOrDefaultAsync<UserDbModel>(cmd);
        return db is null ? null : UserMapper.ToDomain(db);
    }

    public async Task<User[]> GetUserByNameAsync(string name, string surname, CancellationToken ct)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct);
        var cmd = new CommandDefinition(
            "select * from app.fn_get_user_by_name(@p_name, @p_surname);",
            new { p_name = name, p_surname = surname },
            cancellationToken: ct
        );
        var rows = await conn.QueryAsync<UserDbModel>(cmd);
        return rows.Select(UserMapper.ToDomain).ToArray();
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken ct)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct);
        var cmd = new CommandDefinition(
            "select * from app.fn_update_user(@p_id, @p_password, @p_name, @p_surname, @p_age);",
            new
            {
                p_id = user.Id,
                p_password = user.Password,
                p_name = user.Name,
                p_surname = user.Surname,
                p_age = user.Age
            },
            cancellationToken: ct
        );
        var db = await conn.QuerySingleAsync<UserDbModel>(cmd);
        return UserMapper.ToDomain(db);
    }

    public async Task DeleteUserAsync(int id, CancellationToken ct)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct);
        var cmd = new CommandDefinition(
            "select app.fn_delete_user(@p_id);",
            new { p_id = id },
            cancellationToken: ct
        );
        await conn.ExecuteAsync(cmd);
    }
}
