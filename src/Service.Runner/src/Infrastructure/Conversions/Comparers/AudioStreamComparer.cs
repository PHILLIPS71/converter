using Xabe.FFmpeg;

namespace Giantnodes.Service.Runner.Infrastructure.Conversions;

internal sealed class AudioStreamComparer : IComparer<IAudioStream>
{
    private readonly AudioStreamConfiguration _configuration;

    public AudioStreamComparer(AudioStreamConfiguration configuration)
    {
        _configuration = configuration;
    }

    public int Compare(IAudioStream? x, IAudioStream? y)
    {
        if (x == null || y == null)
        {
            return (x == null, y == null) switch
            {
                (true, true) => 0,
                (true, false) => -1,
                (false, true) => 1,
                (false, false) => 0
            };
        }

        if (!string.IsNullOrEmpty(_configuration.Language))
        {
            var left = x.Language.Equals(_configuration.Language, StringComparison.OrdinalIgnoreCase);
            var right = y.Language.Equals(_configuration.Language, StringComparison.OrdinalIgnoreCase);

            if (left != right)
                return left ? -1 : 1;
        }

        if (_configuration.Channels.HasValue)
        {
            var left = Math.Abs(x.Channels - _configuration.Channels.Value);
            var right = Math.Abs(y.Channels - _configuration.Channels.Value);

            if (left != right)
                return left.CompareTo(right);
        }

        if (_configuration.Bitrate.HasValue)
        {
            var left = Math.Abs(x.Bitrate - _configuration.Bitrate.Value);
            var right = Math.Abs(y.Bitrate - _configuration.Bitrate.Value);

            if (left != right)
                return left.CompareTo(right);

            return 0;
        }

        return y.Bitrate.CompareTo(x.Bitrate);
    }
}