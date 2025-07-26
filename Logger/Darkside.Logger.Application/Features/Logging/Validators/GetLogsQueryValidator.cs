using FluentValidation;

public class GetLogsQueryValidator : AbstractValidator<GetLogsQuery>
{
    public GetLogsQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotNull()
            .WithMessage("TenantId is required.");
    }
}
