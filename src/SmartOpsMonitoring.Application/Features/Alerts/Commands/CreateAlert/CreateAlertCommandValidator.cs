namespace SmartOpsMonitoring.Application.Features.Alerts.Commands.CreateAlert;

/// <summary>
/// FluentValidation validator for <see cref="CreateAlertCommand"/>.
/// </summary>
public class CreateAlertCommandValidator : AbstractValidator<CreateAlertCommand>
{
    private const int TitleMaxLength = 200;
    private const int MessageMaxLength = 2000;
    private const string HostIdRequiredMessage = "HostId is required.";
    private const string SeverityInvalidMessage = "Severity must be one of: Info, Warning, Critical.";

    /// <summary>Initialises validation rules for <see cref="CreateAlertCommand"/>.</summary>
    public CreateAlertCommandValidator()
    {
        RuleFor(x => x.HostId).NotEmpty().WithMessage(HostIdRequiredMessage);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(TitleMaxLength);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(MessageMaxLength);
        RuleFor(x => x.Severity).IsInEnum();
    }
}
