namespace SmartOpsMonitoring.Application.Features.ServiceNodes.Commands.CreateServiceNode;

/// <summary>
/// FluentValidation validator for <see cref="CreateServiceNodeCommand"/>.
/// </summary>
public class CreateServiceNodeCommandValidator : AbstractValidator<CreateServiceNodeCommand>
{
    private const int NameMaxLength = 200;
    private const int TypeMaxLength = 100;
    private const int PortMin = 1;
    private const int PortMax = 65535;

    /// <summary>Initialises validation rules for <see cref="CreateServiceNodeCommand"/>.</summary>
    public CreateServiceNodeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(NameMaxLength);
        RuleFor(x => x.Type).NotEmpty().MaximumLength(TypeMaxLength);
        RuleFor(x => x.HostId).NotEmpty().WithMessage("HostId is required.");
        RuleFor(x => x.Port)
            .InclusiveBetween(PortMin, PortMax)
            .When(x => x.Port.HasValue)
            .WithMessage($"Port must be between {PortMin} and {PortMax}.");
    }
}
