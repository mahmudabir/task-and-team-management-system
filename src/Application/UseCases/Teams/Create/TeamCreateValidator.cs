using FluentValidation;

namespace Application.UseCases.Teams.Create;

public class TeamCreateValidator : AbstractValidator<TeamCreateCommand>
{
    public TeamCreateValidator()
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