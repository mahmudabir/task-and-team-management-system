using Application.UseCases.Teams.Create;

using FluentValidation;

namespace Application.UseCases.Users.Create;

public class ApplicationUserCreateValidator : AbstractValidator<UserCreateCommand>
{
    public ApplicationUserCreateValidator()
    {
        // Attributes in the model are for EF Core and DbContext validation. not for FluentValidation

        // RuleFor methods are for FluentValidation
        RuleFor(x => x.Payload).NotNull()
                               .DependentRules(() =>
                               {
                                   RuleFor(x => x.Payload.Id).Empty();
                               });
    }
}