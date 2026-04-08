using FluentValidation;

namespace SmartOpsMonitoring.Application.Features.ServiceNodes.Commands.CreateServiceNode;

/// <summary>
/// FluentValidation validator for <see cref="CreateServiceNodeCommand"/>.
/// </summary>
public class CreateServiceNodeCommandValidator : AbstractValidator<CreateServiceNodeCommand>
{
    /// <summary>Initialises validation rules for <see cref="CreateServiceNodeCommand"/>.</summary>
    public CreateServiceNodeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type).NotEmpty().MaximumLength(100);
        RuleFor(x => x.HostId).NotEmpty().WithMessage("HostId is required.");
        RuleFor(x => x.Port)
            .InclusiveBetween(1, 65535)
            .When(x => x.Port.HasValue)
            .WithMessage("Port must be between 1 and 65535.");
    }
}
