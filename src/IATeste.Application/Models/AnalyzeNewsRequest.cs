namespace IATeste.Application.Models;

public sealed record AnalyzeNewsRequest(
    string Title,
    string Description,
    string Region,
    string TagsCsv,
    DateTimeOffset? PublishedAt = null);
