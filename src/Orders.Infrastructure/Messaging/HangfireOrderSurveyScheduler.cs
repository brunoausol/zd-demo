using Hangfire;
using Microsoft.Extensions.Options;
using Orders.Application.Abstractions;
using Orders.Application.Contracts;
using Orders.Infrastructure.Options;

namespace Orders.Infrastructure.Messaging;

public sealed class HangfireOrderSurveyScheduler : IOrderSurveyScheduler
{
    private readonly IBackgroundJobClient _backgroundJobs;
    private readonly HangfireOptions _options;

    public HangfireOrderSurveyScheduler(
        IBackgroundJobClient backgroundJobs,
        IOptions<HangfireOptions> options)
    {
        _backgroundJobs = backgroundJobs;
        _options = options.Value;
    }

    public Task ScheduleAsync(OrderSurveyRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var delayHours = _options.SurveyDelayHours <= 0 ? 24 : _options.SurveyDelayHours;
        _backgroundJobs.Schedule<SendPurchaseSurveyJob>(
            job => job.ExecuteAsync(request),
            TimeSpan.FromHours(delayHours));

        return Task.CompletedTask;
    }
}
