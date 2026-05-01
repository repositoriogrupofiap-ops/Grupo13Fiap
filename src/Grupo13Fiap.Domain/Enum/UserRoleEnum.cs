using System;
namespace Grupo13Fiap.Domain.Enum;

public enum UserRoleEnum
{
    Admin,
    User,
    Publisher,
    Buyer
}

public static class UserRoleEnumExtensions
{
    public static string ToIdentityRole(this UserRoleEnum role) =>
        role.ToString(); // "Admin", "User", "Publisher", "Buyer"

    public static UserRoleEnum ToDomainRole(this string role) =>
        global::System.Enum.Parse<UserRoleEnum>(role, ignoreCase: true);
}