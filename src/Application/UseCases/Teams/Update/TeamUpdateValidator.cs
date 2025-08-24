using FluentValidation;

namespace Application.UseCases.Teams.Update;

public class TeamUpdateValidator : AbstractValidator<TeamUpdateCommand>
{
    public TeamUpdateValidator()
    {
        // Attributes in the model are for EF Core and DbContext validation. not for FluentValidation

        // RuleFor methods are for FluentValidation
        RuleFor(x => x.Payload).NotNull();
        RuleFor(x => x.Payload.Id).NotEmpty();
        RuleFor(x => x.Payload.Description).NotEmpty();
    }
}