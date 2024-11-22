using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Values;
using FileStream = Giantnodes.Service.Supervisor.Domain.Values.FileStream;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;

public sealed class FileSystemFile : FileSystemEntry
{
    private FileSystemFile()
    {
    }

    internal FileSystemFile(PathInfo path, long size, FileSystemDirectory? parent = null)
        : base(path, size, parent)
    {
    }

    public void SetStreams(params FileStream[] streams)
    {
        VideoStreams = VideoStreams
            .Union(streams.OfType<VideoStream>())
            .Intersect(streams.OfType<VideoStream>())
            .ToList();

        AudioStreams = AudioStreams
            .Union(streams.OfType<AudioStream>())
            .Intersect(streams.OfType<AudioStream>())
            .ToList();

        SubtitleStreams = SubtitleStreams
            .Union(streams.OfType<SubtitleStream>())
            .Intersect(streams.OfType<SubtitleStream>())
            .ToList();
    }

    public IReadOnlyCollection<VideoStream> VideoStreams { get; private set; } = new List<VideoStream>();

    public IReadOnlyCollection<AudioStream> AudioStreams { get; private set; } = new List<AudioStream>();

    public IReadOnlyCollection<SubtitleStream> SubtitleStreams { get; private set; } = new List<SubtitleStream>();
}