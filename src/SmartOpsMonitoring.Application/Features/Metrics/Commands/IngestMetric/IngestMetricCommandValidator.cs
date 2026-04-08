namespace SmartOpsMonitoring.Application.Features.Metrics.Commands.IngestMetric;

/// <summary>
/// FluentValidation validator for <see cref="IngestMetricCommand"/>.
/// </summary>
public class IngestMetricCommandValidator : AbstractValidator<IngestMetricCommand>
{
    private const int MetricTypeMaxLength = 100;
    private const int UnitMaxLength = 50;

    /// <summary>Initialises validation rules for <see cref="IngestMetricCommand"/>.</summary>
    public IngestMetricCommandValidator()
    {
        RuleFor(x => x.HostId).NotEmpty().WithMessage("HostId is required.");
        RuleFor(x => x.MetricType).NotEmpty().MaximumLength(MetricTypeMaxLength);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(UnitMaxLength);
        RuleFor(x => x.Value).Must(v => !double.IsNaN(v) && !double.IsInfinity(v))
            .WithMessage("Value must be a finite number.");
    }
}
