using IATeste.Application.Models;

namespace IATeste.Application.Abstractions;

public interface IInvestmentSignalService
{
    Task<AnalyzeNewsResponse> AnalyzeNewsAsync(AnalyzeNewsRequest request, CancellationToken cancellationToken = default);
}
