namespace Orders.Domain.Entities;

public sealed class Order
{
    private Order()
    { }

    private Order(Guid id, string customerName, decimal totalAmount)
    {
        Id = id;
        CustomerName = customerName;
        TotalAmount = totalAmount;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public DateTime CreatedAtUtc { get; private set; }
    public string CustomerName { get; private set; }
    public Guid Id { get; private set; }
    public decimal TotalAmount { get; private set; }

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