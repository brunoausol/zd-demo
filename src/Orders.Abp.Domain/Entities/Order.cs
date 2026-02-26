using Volo.Abp.Domain.Entities.Auditing;

namespace Orders.Abp.Domain.Entities;

public class Order : CreationAuditedAggregateRoot<Guid>
{
    public string CustomerName { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }

    protected Order()
    {
    }

    private Order(Guid id, string customerName, decimal totalAmount) : base(id)
    {
        CustomerName = customerName;
        TotalAmount = totalAmount;
    }

    public static Order Create(string customerName, decimal totalAmount)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("Customer name is required.", nameof(customerName));
        }

        if (totalAmount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalAmount), "Total amount must be positive.");
        }

        return new Order(Guid.NewGuid(), customerName.Trim(), totalAmount);
    }
}
