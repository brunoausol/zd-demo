namespace Orders.Infrastructure.Options;

public sealed class HangfireOptions
{
    public const string SectionName = "Hangfire";

    public string DashboardPath { get; init; } = "/hangfire";
    public int WorkerCount { get; init; } = 5;
    public int SurveyDelayHours { get; init; } = 24;
    public int ShutdownTimeoutSeconds { get; init; } = 30;
}
