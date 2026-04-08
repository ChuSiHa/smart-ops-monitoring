using FluentValidation;
using SmartOpsMonitoring.Domain.Enums;

namespace SmartOpsMonitoring.Application.Features.Alerts.Commands.CreateAlert;

/// <summary>
/// FluentValidation validator for <see cref="CreateAlertCommand"/>.
/// </summary>
public class CreateAlertCommandValidator : AbstractValidator<CreateAlertCommand>
{
    /// <summary>Initialises validation rules for <see cref="CreateAlertCommand"/>.</summary>
    public CreateAlertCommandValidator()
    {
        RuleFor(x => x.HostId).NotEmpty().WithMessage("HostId is required.");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Severity)
            .NotEmpty()
            .Must(s => Enum.TryParse<AlertSeverity>(s, true, out _))
            .WithMessage("Severity must be one of: Info, Warning, Critical.");
    }
}
