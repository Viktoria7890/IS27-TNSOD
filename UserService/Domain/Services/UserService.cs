using FluentValidation;
using UserService.Data.Repositories;
using UserService.Domain.Exceptions;
using UserService.Domain.Models;
using UserService.Domain.Validators;

namespace UserService.Domain.Services;

public class UserService : IUserService
{
    private readonly IValidator<User> _createValidator;
    private readonly IUserRepository _repository;
    private readonly IValidator<User> _updateValidator;

    public UserService(
        IUserRepository repository,
        CreateUserValidator createValidator,
        UpdateUserValidator updateValidator)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<User> CreateUserAsync(User user, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(user, ct);

        if (await _repository.IsLoginExistsAsync(user.Login, ct))
            throw new UserAlreadyExistsException(user.Login);

        return await _repository.CreateUserAsync(user, ct);
    }

    public async Task<User> GetUserByIdAsync(int id, CancellationToken ct)
    {
        var user = await _repository.GetUserByIdAsync(id, ct);
        if (user == null)
            throw new UserNotFoundException(id);

        return user;
    }

    public async Task<User[]> GetUserByNameAsync(string name, string surname, CancellationToken ct)
    {
        return await _repository.GetUserByNameAsync(name, surname, ct);
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(user, ct);

        var existing = await _repository.GetUserByIdAsync(user.Id, ct);
        if (existing is null) throw new UserNotFoundException(user.Id);

        return await _repository.UpdateUserAsync(user, ct);
    }

    public async Task DeleteUserAsync(int id, CancellationToken ct)
    {
        var existing = await _repository.GetUserByIdAsync(id, ct);
        if (existing is null) throw new UserNotFoundException(id);
        await _repository.DeleteUserAsync(id, ct);
    }
}