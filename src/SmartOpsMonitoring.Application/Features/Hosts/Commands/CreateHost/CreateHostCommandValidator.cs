namespace SmartOpsMonitoring.Application.Features.Hosts.Commands.CreateHost;

/// <summary>
/// FluentValidation validator for <see cref="CreateHostCommand"/>.
/// </summary>
public class CreateHostCommandValidator : AbstractValidator<CreateHostCommand>
{
    private const int NameMaxLength = 200;
    private const int OsTypeMaxLength = 100;
    private const int IpAddressMaxLength = 45;

    /// <summary>Initialises validation rules for <see cref="CreateHostCommand"/>.</summary>
    public CreateHostCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(NameMaxLength);
        RuleFor(x => x.OsType).NotEmpty().MaximumLength(OsTypeMaxLength);
        RuleFor(x => x.IpAddress)
            .MaximumLength(IpAddressMaxLength)
            .Must(ip => string.IsNullOrEmpty(ip) || System.Net.IPAddress.TryParse(ip, out _))
            .WithMessage("IpAddress must be a valid IP address.");
    }
}
