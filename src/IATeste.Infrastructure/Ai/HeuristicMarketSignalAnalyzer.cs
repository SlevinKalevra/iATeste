using IATeste.Application.Abstractions;
using IATeste.Domain.Entities;
using IATeste.Domain.Enums;

namespace IATeste.Infrastructure.Ai;

public sealed class HeuristicMarketSignalAnalyzer : IMarketSignalAnalyzer
{
    private static readonly (string[] Keywords, string Asset, SignalDirection Direction, decimal Confidence, string Reason)[] Rules =
    [
        ( ["guerra", "conflito", "ataque", "militar", "tensão"], "Petróleo (Brent)", SignalDirection.Up, 0.73m, "Conflitos geopolíticos elevam prêmio de risco energético" ),
        ( ["guerra", "sanção", "crise", "instabilidade"], "Ouro", SignalDirection.Up, 0.68m, "Ativo de proteção em cenários de risco" ),
        ( ["sanção", "defesa", "militar"], "Setor de Defesa", SignalDirection.Up, 0.62m, "Possível aumento de gastos governamentais" ),
        ( ["seca", "enchente", "safra", "clima", "furacão"], "Commodities Agrícolas", SignalDirection.Up, 0.66m, "Risco de choque de oferta agrícola" ),
        ( ["desastre", "terremoto", "vulcão", "enchente"], "Seguradoras", SignalDirection.Down, 0.54m, "Maior sinistralidade esperada" ),
        ( ["juros", "inflação", "banco central"], "Tecnologia Growth", SignalDirection.Down, 0.57m, "Sensível ao custo de capital" ),
        ( ["logística", "porto", "marítimo", "bloqueio"], "Frete Marítimo", SignalDirection.Up, 0.65m, "Disrupção logística tende a elevar preços" )
    ];

    public Task<IReadOnlyCollection<AssetSignal>> AnalyzeAsync(NewsEvent newsEvent, IReadOnlyCollection<HistoricalContext> historicalContexts, CancellationToken cancellationToken = default)
    {
        var normalizedText = $"{newsEvent.Title} {newsEvent.Description} {newsEvent.Region} {string.Join(' ', newsEvent.Tags)}".ToLowerInvariant();

        var candidateSignals = Rules
            .Where(rule => rule.Keywords.Any(normalizedText.Contains))
            .Select(rule => new AssetSignal(rule.Asset, rule.Direction, rule.Confidence, rule.Reason))
            .ToList();

        if (historicalContexts.Count > 0)
        {
            var historicalBoost = historicalContexts
                .SelectMany(x => x.ObservedSignals)
                .GroupBy(x => new { x.Asset, x.Direction, x.Reason })
                .Select(group => new AssetSignal(
                    group.Key.Asset,
                    group.Key.Direction,
                    Math.Min(0.95m, group.Average(x => x.Confidence) * 0.9m),
                    $"Base histórica: {group.Key.Reason}"));

            candidateSignals.AddRange(historicalBoost);
        }

        var consolidated = candidateSignals
            .GroupBy(signal => signal.Asset, StringComparer.OrdinalIgnoreCase)
            .Select(group =>
            {
                var selected = group.OrderByDescending(x => x.Confidence).First();
                return selected with { Confidence = Math.Round(Math.Min(0.95m, selected.Confidence), 2) };
            })
            .OrderByDescending(x => x.Confidence)
            .Take(8)
            .ToArray();

        if (consolidated.Length == 0)
        {
            return Task.FromResult<IReadOnlyCollection<AssetSignal>>(
            [
                new AssetSignal(
                    "Índice Global Diversificado",
                    SignalDirection.Neutral,
                    0.4m,
                    "Sem sinal forte identificado; necessário mais contexto")
            ]);
        }

        return Task.FromResult<IReadOnlyCollection<AssetSignal>>(consolidated);
    }
}
