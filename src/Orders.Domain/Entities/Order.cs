namespace Orders.Domain.Entities;

public sealed class Order
{
    private Order()
    { }

    private Order(Guid id, string firstName, string lastName, string customerName, decimal totalAmount)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        CustomerName = customerName;
        TotalAmount = totalAmount;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public DateTime CreatedAtUtc { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string CustomerName { get; private set; }
    public Guid Id { get; private set; }
    public decimal TotalAmount { get; private set; }

    public static Order Create(string customerName, decimal totalAmount)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("Customer name is required.", nameof(customerName));
        }
        
        var nameParts = customerName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var firstName = nameParts[0];
        var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

        return new Order(Guid.NewGuid(), firstName, lastName, customerName, totalAmount);
    }

    public static Order Create(string firstName, string lastName, decimal totalAmount)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name is required.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name is required.", nameof(lastName));
        }

        if (totalAmount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalAmount), "Total amount must be positive.");
        }

        var normalizedFirstName = firstName.Trim();
        var normalizedLastName = lastName.Trim();
        var customerName = $"{normalizedFirstName} {normalizedLastName}";

        return new Order(Guid.NewGuid(), normalizedFirstName, normalizedLastName, customerName, totalAmount);
    }
}
