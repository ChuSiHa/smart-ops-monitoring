using FluentValidation;

namespace SmartOpsMonitoring.Application.Features.Metrics.Commands.IngestMetric;

/// <summary>
/// FluentValidation validator for <see cref="IngestMetricCommand"/>.
/// </summary>
public class IngestMetricCommandValidator : AbstractValidator<IngestMetricCommand>
{
    /// <summary>Initialises validation rules for <see cref="IngestMetricCommand"/>.</summary>
    public IngestMetricCommandValidator()
    {
        RuleFor(x => x.HostId).NotEmpty().WithMessage("HostId is required.");
        RuleFor(x => x.MetricType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Value).Must(v => !double.IsNaN(v) && !double.IsInfinity(v))
            .WithMessage("Value must be a finite number.");
    }
}
