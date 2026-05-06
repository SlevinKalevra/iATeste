using IATeste.Application.Abstractions;
using IATeste.Application.Models;
using IATeste.Application.UseCases;
using IATeste.Domain.Entities;
using IATeste.Domain.Enums;

namespace IATeste.Application.Tests.UseCases;

public sealed class InvestmentSignalServiceTests
{
    [Fact]
    public async Task AnalyzeNewsAsync_ShouldReturnSignalsAndContexts()
    {
        var sut = new InvestmentSignalService(new FakeRetriever(), new FakeAnalyzer());

        var response = await sut.AnalyzeNewsAsync(new AnalyzeNewsRequest(
            Title: "Teste",
            Description: "Descrição relevante",
            Region: "Oriente Médio",
            TagsCsv: "guerra,energia"));

        Assert.Single(response.Signals);
        Assert.Single(response.HistoricalContexts);
        Assert.Equal("Petróleo (Brent)", response.Signals.First().Asset);
    }

    [Fact]
    public async Task AnalyzeNewsAsync_ShouldThrow_WhenInvalidInput()
    {
        var sut = new InvestmentSignalService(new FakeRetriever(), new FakeAnalyzer());

        await Assert.ThrowsAsync<ArgumentException>(() => sut.AnalyzeNewsAsync(new AnalyzeNewsRequest(
            Title: " ",
            Description: "desc",
            Region: "região",
            TagsCsv: "")));
    }

    private sealed class FakeRetriever : IHistoricalContextRetriever
    {
        public Task<IReadOnlyCollection<HistoricalContext>> RetrieveSimilarEventsAsync(NewsEvent newsEvent, int limit, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<HistoricalContext> result =
            [
                new HistoricalContext(
                    "Evento passado",
                    "Oriente Médio",
                    DateTimeOffset.UtcNow.AddYears(-1),
                    [new AssetSignal("Petróleo (Brent)", SignalDirection.Up, 0.7m, "Histórico")],
                    "Resumo")
            ];

            return Task.FromResult(result);
        }
    }

    private sealed class FakeAnalyzer : IMarketSignalAnalyzer
    {
        public Task<IReadOnlyCollection<AssetSignal>> AnalyzeAsync(NewsEvent newsEvent, IReadOnlyCollection<HistoricalContext> historicalContexts, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<AssetSignal> signals =
            [
                new AssetSignal("Petróleo (Brent)", SignalDirection.Up, 0.8m, "Regra heurística")
            ];

            return Task.FromResult(signals);
        }
    }
}
