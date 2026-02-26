namespace Orders.Abp.Application.Options;

public class OrderSurveyOptions
{
    public const string SectionName = "OrderSurvey";

    public int DelayHours { get; set; } = 24;
}
