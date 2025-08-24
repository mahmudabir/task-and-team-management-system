using FluentValidation;

namespace Application.UseCases.Teams.Update;

public class TeamAssignUserValidator : AbstractValidator<TeamAssignUserCommand>
{
    public TeamAssignUserValidator()
    {
        // Attributes in the model are for EF Core and DbContext validation. not for FluentValidation

        // RuleFor methods are for FluentValidation
        RuleFor(x => x.Payload).NotNull()
                               .DependentRules(() =>
                               {
                                   RuleFor(x => x.Payload).NotEmpty();
                               });
    }
}