using Orders.Application.Contracts;

namespace Orders.Application.Abstractions;

public interface IOrderSurveyScheduler
{
    Task ScheduleAsync(OrderSurveyRequest request, CancellationToken cancellationToken);
}
