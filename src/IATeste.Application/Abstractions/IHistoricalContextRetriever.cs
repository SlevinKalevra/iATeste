using IATeste.Domain.Entities;

namespace IATeste.Application.Abstractions;

public interface IHistoricalContextRetriever
{
    Task<IReadOnlyCollection<HistoricalContext>> RetrieveSimilarEventsAsync(NewsEvent newsEvent, int limit, CancellationToken cancellationToken = default);
}
