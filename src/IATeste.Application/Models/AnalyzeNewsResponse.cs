using IATeste.Domain.Entities;

namespace IATeste.Application.Models;

public sealed record AnalyzeNewsResponse(
    IReadOnlyCollection<AssetSignal> Signals,
    IReadOnlyCollection<HistoricalContext> HistoricalContexts);
