namespace Orders.Abp.HttpApi.Host.Options;

public class HangfireDashboardOptions
{
    public const string SectionName = "Hangfire";

    public string DashboardPath { get; set; } = "/hangfire";
}
