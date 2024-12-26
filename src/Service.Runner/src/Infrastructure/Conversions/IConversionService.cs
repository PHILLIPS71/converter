using ErrorOr;

namespace Giantnodes.Service.Runner.Infrastructure.Conversions;

internal interface IConversionService
{
    Task<ErrorOr<Success>> ConvertAsync(
        string path,
        string? extension,
        ICollection<VideoStreamConfiguration> video,
        ICollection<AudioStreamConfiguration> audio,
        ICollection<SubtitleStreamConfiguration> subtitle,
        CancellationToken cancellation = default);
}