using ErrorOr;
using System.IO.Abstractions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Enumerations;

namespace Giantnodes.Service.Supervisor.Domain.Values;

public sealed record PathInfo : ValueObject
{
    private PathInfo()
    {
    }

    private PathInfo(IFileSystemInfo info)
    {
        Name = info.Name;
        FullName = info.FullName;
        DirectoryPath = Path.GetDirectoryName(info.FullName);
        DirectorySeparatorChar = Path.DirectorySeparatorChar;
        Container = string.IsNullOrWhiteSpace(info.Extension)
            ? null
            : Enumeration.Parse<VideoFileContainer>(x => x.Extension == info.Extension);
    }

    public static ErrorOr<PathInfo> Create(IFileSystemInfo info)
    {
        if (!string.IsNullOrWhiteSpace(info.Extension) &&
            !Enumeration.TryParse<VideoFileContainer>(x => x.Extension == info.Extension, out _))
        {
            return Error.Validation(description: $"file extension '{info.Extension}' is not supported");
        }

        return new PathInfo(info);
    }

    public string Name { get; init; }

    public string FullName { get; init; }

    public VideoFileContainer? Container { get; init; }

    public string? DirectoryPath { get; init; }

    public char DirectorySeparatorChar { get; init; }
}