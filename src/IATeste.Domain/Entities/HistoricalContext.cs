namespace IATeste.Domain.Entities;

public sealed record HistoricalContext(
    string Event,
    string Region,
    DateTimeOffset Date,
    IReadOnlyCollection<AssetSignal> ObservedSignals,
    string Summary);
