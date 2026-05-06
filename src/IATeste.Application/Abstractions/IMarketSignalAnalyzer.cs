using IATeste.Domain.Entities;

namespace IATeste.Application.Abstractions;

public interface IMarketSignalAnalyzer
{
    Task<IReadOnlyCollection<AssetSignal>> AnalyzeAsync(NewsEvent newsEvent, IReadOnlyCollection<HistoricalContext> historicalContexts, CancellationToken cancellationToken = default);
}
