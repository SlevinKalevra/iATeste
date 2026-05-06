using IATeste.Application.Abstractions;
using IATeste.Domain.Entities;

namespace IATeste.Infrastructure.Rag;

public sealed class InMemoryHistoricalContextRetriever : IHistoricalContextRetriever
{
    private static readonly IReadOnlyCollection<HistoricalContext> HistoricalData =
    [
        new(
            Event: "Conflito no Golfo Pérsico",
            Region: "Oriente Médio",
            Date: new DateTimeOffset(2020, 1, 3, 0, 0, 0, TimeSpan.Zero),
            ObservedSignals:
            [
                new AssetSignal("Petróleo (Brent)", Domain.Enums.SignalDirection.Up, 0.82m, "Risco de oferta global"),
                new AssetSignal("Ouro", Domain.Enums.SignalDirection.Up, 0.76m, "Movimento de proteção"),
                new AssetSignal("Companhias aéreas", Domain.Enums.SignalDirection.Down, 0.63m, "Aumento de custo de combustível")
            ],
            Summary: "Tensão militar elevou risco geopolítico e preço de energia."),
        new(
            Event: "Escalada da guerra no Leste Europeu",
            Region: "Europa",
            Date: new DateTimeOffset(2022, 2, 24, 0, 0, 0, TimeSpan.Zero),
            ObservedSignals:
            [
                new AssetSignal("Gás natural", Domain.Enums.SignalDirection.Up, 0.88m, "Risco energético europeu"),
                new AssetSignal("Defesa", Domain.Enums.SignalDirection.Up, 0.71m, "Aumento de demanda governamental"),
                new AssetSignal("Índices europeus", Domain.Enums.SignalDirection.Down, 0.58m, "Risco macro e aversão")
            ],
            Summary: "Sanções e disrupções elevaram commodities e defesa."),
        new(
            Event: "Seca severa e quebra de safra",
            Region: "América do Sul",
            Date: new DateTimeOffset(2021, 8, 12, 0, 0, 0, TimeSpan.Zero),
            ObservedSignals:
            [
                new AssetSignal("Soja", Domain.Enums.SignalDirection.Up, 0.74m, "Oferta menor de grãos"),
                new AssetSignal("Milho", Domain.Enums.SignalDirection.Up, 0.69m, "Pressão na cadeia alimentar"),
                new AssetSignal("Frigoríficos", Domain.Enums.SignalDirection.Down, 0.55m, "Custo de ração maior")
            ],
            Summary: "Choques climáticos pressionaram commodities agrícolas."),
        new(
            Event: "Ataques em rotas marítimas comerciais",
            Region: "Mar Vermelho",
            Date: new DateTimeOffset(2024, 1, 10, 0, 0, 0, TimeSpan.Zero),
            ObservedSignals:
            [
                new AssetSignal("Frete marítimo", Domain.Enums.SignalDirection.Up, 0.8m, "Rota alternativa e custos maiores"),
                new AssetSignal("Petróleo (WTI)", Domain.Enums.SignalDirection.Up, 0.61m, "Prêmio de risco logístico")
            ],
            Summary: "Risco logístico elevou custo de transporte e energia.")
    ];

    public Task<IReadOnlyCollection<HistoricalContext>> RetrieveSimilarEventsAsync(NewsEvent newsEvent, int limit, CancellationToken cancellationToken = default)
    {
        var tokens = BuildTokens(newsEvent.Title, newsEvent.Description, newsEvent.Region, string.Join(' ', newsEvent.Tags));

        var results = HistoricalData
            .Select(context => new
            {
                Context = context,
                Score = BuildTokens(context.Event, context.Region, context.Summary)
                    .Intersect(tokens, StringComparer.OrdinalIgnoreCase)
                    .Count()
            })
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Context.Date)
            .Take(limit)
            .Where(x => x.Score > 0)
            .Select(x => x.Context)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<HistoricalContext>>(results);
    }

    private static HashSet<string> BuildTokens(params string[] fragments)
    {
        return fragments
            .Where(f => !string.IsNullOrWhiteSpace(f))
            .SelectMany(f => f.Split([' ', ',', '.', ';', ':', '-', '_', '/'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(f => f.Length > 2)
            .Select(f => f.ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
