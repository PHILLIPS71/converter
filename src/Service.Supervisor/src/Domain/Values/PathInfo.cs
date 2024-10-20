using System.IO.Abstractions;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Values;

public sealed record PathInfo : ValueObject
{
    private PathInfo()
    {
    }

    public PathInfo(IFileSystemInfo info)
    {
        Name = info.Name;
        FullName = info.FullName;
        Extension = string.IsNullOrWhiteSpace(info.Extension) ? null : info.Extension;
        DirectoryPath = Path.GetDirectoryName(info.FullName);
        DirectorySeparatorChar = Path.DirectorySeparatorChar;
    }

    public string Name { get; init; }

    public string FullName { get; init; }

    public string? Extension { get; init; }

    public string? DirectoryPath { get; init; }

    public char DirectorySeparatorChar { get; init; }
}