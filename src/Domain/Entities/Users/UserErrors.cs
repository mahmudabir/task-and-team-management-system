using Softoverse.CqrsKit.Models.Utility;

namespace Domain.Entities.Users;

public static class UserErrors
{
    public static CqrsError NotFound(string userId) => CqrsError.Create(
                                                                        "Users.NotFound",
                                                                        $"The user with the Id = '{userId}' was not found");

    public static CqrsError Unauthorized() => CqrsError.Create(
                                                                  "Users.Unauthorized",
                                                                  "You are not authorized to perform this action.");

    public static readonly CqrsError NotFoundByEmail = CqrsError.Create(
                                                                           "Users.NotFoundByEmail",
                                                                           "The user with the specified email was not found");

    public static readonly CqrsError EmailNotUnique = CqrsError.Create(
                                                                          "Users.EmailNotUnique",
                                                                          "The provided email is not unique");

    public static readonly CqrsError AlreadyExists = CqrsError.Create(
                                                                       "Users.AlreadyExists",
                                                                       "User already exists");
}
