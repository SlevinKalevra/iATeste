using IATeste.Domain.Enums;

namespace IATeste.Domain.Entities;

public sealed record AssetSignal(
    string Asset,
    SignalDirection Direction,
    decimal Confidence,
    string Reason);
