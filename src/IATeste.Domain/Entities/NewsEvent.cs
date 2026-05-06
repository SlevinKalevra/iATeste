namespace IATeste.Domain.Entities;

public sealed record NewsEvent(
    string Title,
    string Description,
    string Region,
    DateTimeOffset PublishedAt,
    IReadOnlyCollection<string> Tags);
