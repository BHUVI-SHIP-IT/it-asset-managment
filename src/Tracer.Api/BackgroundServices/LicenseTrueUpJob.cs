using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.LicenseAggregate;

namespace Tracer.Api.BackgroundServices;

/// <summary>
/// Hangfire job for auditing license usage against purchased seats.
/// </summary>
public sealed class LicenseTrueUpJob
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<LicenseTrueUpJob> _logger;

    public LicenseTrueUpJob(IApplicationDbContext context, ILogger<LicenseTrueUpJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting License True-Up Job");

        // Simple true-up logic for demonstration:
        // Audit all licenses to ensure allocated seats <= total seats.
        var licenses = await _context.SoftwareLicenses.ToListAsync(cancellationToken);

        foreach (var license in licenses)
        {
            var allocatedSeatsCount = await _context.LicenseSeats
                .CountAsync(s => s.SoftwareLicenseId == license.Id, cancellationToken);

            if (allocatedSeatsCount > license.TotalSeats)
            {
                // In a real scenario, this might dispatch a domain event to notify administrators.
                _logger.LogWarning("License True-Up Alert: License {LicenseName} (ID: {LicenseId}) has {Allocated} allocated seats, but only {Total} are purchased.",
                    license.Name, license.Id, allocatedSeatsCount, license.TotalSeats);
            }
        }

        _logger.LogInformation("Completed License True-Up Job");
    }
}
