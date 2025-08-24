using Application.UseCases.Teams.Update;

using FluentValidation;

namespace Application.UseCases.Users.Update;

public class UserUpdateValidator : AbstractValidator<UserUpdateCommand>
{
    public UserUpdateValidator()
    {
        RuleFor(x => x.Payload).NotNull()
                               .DependentRules(() =>
                               {
                                   RuleFor(x => x.Payload.Id).NotEmpty();
                                   RuleFor(x => x.Payload.Username).NotEmpty();
                                   RuleFor(x => x.Payload.Email).NotEmpty();
                               });
    }
}