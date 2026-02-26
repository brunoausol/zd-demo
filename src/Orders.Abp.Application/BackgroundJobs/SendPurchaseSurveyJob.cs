using Orders.Abp.Application.Contracts.Orders;
using Orders.Abp.Application.Messaging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Orders.Abp.Application.BackgroundJobs;

public class SendPurchaseSurveyJob : AsyncBackgroundJob<SendPurchaseSurveyJobArgs>, ITransientDependency
{
    private readonly IPurchaseSurveySender _surveySender;

    public SendPurchaseSurveyJob(IPurchaseSurveySender surveySender)
    {
        _surveySender = surveySender;
    }

    public override async Task ExecuteAsync(SendPurchaseSurveyJobArgs args)
    {
        var request = new OrderSurveyRequest(args.OrderId, args.CustomerName, args.OrderCreatedAtUtc);
        await _surveySender.SendAsync(request);
    }
}
