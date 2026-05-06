using IATeste.Application.Abstractions;
using IATeste.Application.UseCases;
using IATeste.Infrastructure.Ai;
using IATeste.Infrastructure.Rag;
using Microsoft.Extensions.DependencyInjection;

namespace IATeste.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInvestmentSignalsPlatform(this IServiceCollection services)
    {
        services.AddScoped<IInvestmentSignalService, InvestmentSignalService>();
        services.AddScoped<IHistoricalContextRetriever, InMemoryHistoricalContextRetriever>();
        services.AddScoped<IMarketSignalAnalyzer, HeuristicMarketSignalAnalyzer>();

        return services;
    }
}
