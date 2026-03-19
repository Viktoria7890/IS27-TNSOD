using Riok.Mapperly.Abstractions;
using UserService.Data.DbModels;
using UserService.Domain.Models;

namespace UserService.Mappings;

[Mapper]
public static partial class UserMapper
{
    public static partial User ToDomain(UserDbModel db);
    public static partial UserDbModel ToDb(User domain);
}
