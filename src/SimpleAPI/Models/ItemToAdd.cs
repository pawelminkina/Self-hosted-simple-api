namespace SimpleAPI.Models;

public record ItemToAdd
{
    public double NumberValue { get; init; }
    public string Name { get; init; } = string.Empty;
}