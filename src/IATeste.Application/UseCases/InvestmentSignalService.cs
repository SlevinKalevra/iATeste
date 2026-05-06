using IATeste.Application.Abstractions;
using IATeste.Application.Models;
using IATeste.Domain.Entities;

namespace IATeste.Application.UseCases;

public sealed class InvestmentSignalService(
    IHistoricalContextRetriever historicalContextRetriever,
    IMarketSignalAnalyzer marketSignalAnalyzer) : IInvestmentSignalService
{
    private const int DefaultContextLimit = 5;

    public async Task<AnalyzeNewsResponse> AnalyzeNewsAsync(AnalyzeNewsRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Title);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Description);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Region);

        var tags = request.TagsCsv
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var newsEvent = new NewsEvent(
            request.Title.Trim(),
            request.Description.Trim(),
            request.Region.Trim(),
            request.PublishedAt ?? DateTimeOffset.UtcNow,
            tags);

        var contexts = await historicalContextRetriever
            .RetrieveSimilarEventsAsync(newsEvent, DefaultContextLimit, cancellationToken)
            .ConfigureAwait(false);

        var signals = await marketSignalAnalyzer
            .AnalyzeAsync(newsEvent, contexts, cancellationToken)
            .ConfigureAwait(false);

        return new AnalyzeNewsResponse(signals, contexts);
    }
}
