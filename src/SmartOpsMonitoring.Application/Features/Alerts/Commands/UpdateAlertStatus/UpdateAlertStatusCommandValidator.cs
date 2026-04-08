using FluentValidation;
using SmartOpsMonitoring.Domain.Enums;

namespace SmartOpsMonitoring.Application.Features.Alerts.Commands.UpdateAlertStatus;

/// <summary>
/// FluentValidation validator for <see cref="UpdateAlertStatusCommand"/>.
/// </summary>
public class UpdateAlertStatusCommandValidator : AbstractValidator<UpdateAlertStatusCommand>
{
    /// <summary>Initialises validation rules for <see cref="UpdateAlertStatusCommand"/>.</summary>
    public UpdateAlertStatusCommandValidator()
    {
        RuleFor(x => x.AlertId).NotEmpty().WithMessage("AlertId is required.");
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(s => Enum.TryParse<AlertStatus>(s, true, out _))
            .WithMessage("Status must be one of: Open, Acknowledged, Resolved.");
    }
}
