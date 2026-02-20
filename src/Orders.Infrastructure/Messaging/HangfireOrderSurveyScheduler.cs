using Hangfire;
using Microsoft.Extensions.Options;
using Orders.Application.Abstractions;
using Orders.Application.Contracts;
using Orders.Infrastructure.Messaging.Envelopes;
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
        var envelope = HangfireEnvelope.Create(HangfireMessageTypes.SendPurchaseSurveyV1, request);
        _backgroundJobs.Schedule<ProcessHangfireEnvelopeJob>(
            job => job.ExecuteAsync(envelope),
            TimeSpan.FromHours(delayHours));

        return Task.CompletedTask;
    }
}
