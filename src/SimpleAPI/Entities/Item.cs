namespace SimpleAPI.Entities;

public record Item
{
    public Guid Id { get; set; }
    public double NumberValue { get; init; }
    public string Name { get; init; } = string.Empty;
}