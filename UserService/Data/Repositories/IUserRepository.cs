using UserService.Domain.Models;

namespace UserService.Data.Repositories;

public interface IUserRepository
{
    Task<bool> IsLoginExistsAsync(string login, CancellationToken ct);

    Task<User> CreateUserAsync(User user, CancellationToken ct);
    Task<User?> GetUserByIdAsync(int id, CancellationToken ct);
    Task<User[]> GetUserByNameAsync(string name, string surname, CancellationToken ct);
    Task<User> UpdateUserAsync(User user, CancellationToken ct);
    Task DeleteUserAsync(int id, CancellationToken ct);
}