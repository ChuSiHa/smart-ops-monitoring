namespace SmartOpsMonitoring.Application.Features.Alerts.Commands.UpdateAlertStatus;

/// <summary>
/// FluentValidation validator for <see cref="UpdateAlertStatusCommand"/>.
/// </summary>
public class UpdateAlertStatusCommandValidator : AbstractValidator<UpdateAlertStatusCommand>
{
    private const int StatusMaxLength = 50;
    private const string AlertIdRequiredMessage = "AlertId is required.";
    private const string StatusInvalidMessage = "Status must be one of: Open, Acknowledged, Resolved.";

    /// <summary>Initialises validation rules for <see cref="UpdateAlertStatusCommand"/>.</summary>
    public UpdateAlertStatusCommandValidator()
    {
        RuleFor(x => x.AlertId).NotEmpty().WithMessage(AlertIdRequiredMessage);
        RuleFor(x => x.Status)
            .NotEmpty()
            .MaximumLength(StatusMaxLength)
            .Must(s => Enum.TryParse<AlertStatus>(s, true, out _))
            .WithMessage(StatusInvalidMessage);
    }
}
