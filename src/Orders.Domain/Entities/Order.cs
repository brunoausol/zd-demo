namespace Orders.Domain.Entities;

public sealed class Order
{
    private Order()
    { }

    private Order(Guid id, string firstName, string lastName, decimal totalAmount)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        TotalAmount = totalAmount;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public DateTime CreatedAtUtc { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Guid Id { get; private set; }
    public decimal TotalAmount { get; private set; }

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

        return new Order(Guid.NewGuid(), normalizedFirstName, normalizedLastName, totalAmount);
    }
}
