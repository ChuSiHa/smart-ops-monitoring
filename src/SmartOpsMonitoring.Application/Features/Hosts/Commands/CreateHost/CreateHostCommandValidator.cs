using FluentValidation;

namespace SmartOpsMonitoring.Application.Features.Hosts.Commands.CreateHost;

/// <summary>
/// FluentValidation validator for <see cref="CreateHostCommand"/>.
/// </summary>
public class CreateHostCommandValidator : AbstractValidator<CreateHostCommand>
{
    /// <summary>Initialises validation rules for <see cref="CreateHostCommand"/>.</summary>
    public CreateHostCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OsType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.IpAddress)
            .MaximumLength(45)
            .Must(ip => string.IsNullOrEmpty(ip) || System.Net.IPAddress.TryParse(ip, out _))
            .WithMessage("IpAddress must be a valid IP address.");
    }
}
