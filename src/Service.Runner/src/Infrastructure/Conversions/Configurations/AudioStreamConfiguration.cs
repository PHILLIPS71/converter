namespace Giantnodes.Service.Runner.Infrastructure.Conversions;

internal sealed record AudioStreamConfiguration(string Language, int? Channels, long? Bitrate, bool Optional = false);