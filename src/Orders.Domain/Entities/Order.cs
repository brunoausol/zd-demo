namespace Orders.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; }
    public string? CustomerEmail { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Order() { }

    private Order(Guid id, string customerName, string? customerEmail, decimal totalAmount, string currency)
    {
        Id = id;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        TotalAmount = totalAmount;
        Currency = currency;
        Status = OrderStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Order Create(string customerName, string? customerEmail, decimal totalAmount, string currency)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("Customer name is required.", nameof(customerName));
        }

        if (totalAmount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalAmount), "Total amount must be positive.");
        }

        var normalizedCurrency = string.IsNullOrWhiteSpace(currency) ? "EUR" : currency.Trim().ToUpperInvariant();
        return new Order(Guid.NewGuid(), customerName.Trim(), customerEmail?.Trim(), totalAmount, normalizedCurrency);
    }

    public void SetCustomerEmail(string? customerEmail)
    {
        CustomerEmail = string.IsNullOrWhiteSpace(customerEmail) ? null : customerEmail.Trim();
    }
}
