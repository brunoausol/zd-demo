namespace Orders.Abp.Application.BackgroundJobs;

public class SendPurchaseSurveyJobArgs
{
    public Guid OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderCreatedAtUtc { get; set; }
}
