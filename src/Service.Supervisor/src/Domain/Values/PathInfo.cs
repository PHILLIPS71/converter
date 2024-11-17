using System.Globalization;
using ErrorOr;
using System.IO.Abstractions;
using System.Text;
using System.Text.RegularExpressions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Enumerations;

namespace Giantnodes.Service.Supervisor.Domain.Values;

public sealed partial record PathInfo : ValueObject
{
    [GeneratedRegex(@"[^a-zA-Z0-9_\.]")]
    private static partial Regex NormalizeFullPathRegex();

    private PathInfo()
    {
    }

    private PathInfo(IFileSystemInfo info)
    {
        Name = info.Name;
        FullName = info.FullName;
        FullNameNormalized = NormalizePath(info.FullName);
        DirectoryPath = Path.GetDirectoryName(info.FullName);
        DirectorySeparatorChar = Path.DirectorySeparatorChar;
        Container = info is not IDirectoryInfo
            ? Enumeration.Parse<VideoFileContainer>(x => x.Extension == info.Extension)
            : null;
    }

    public static ErrorOr<PathInfo> Create(IFileSystemInfo info)
    {
        if (info is not IDirectoryInfo &&
            !Enumeration.TryParse<VideoFileContainer>(x => x.Extension == info.Extension, out _))
            return Error.Validation(description: $"file extension '{info.Extension}' is not supported");

        return new PathInfo(info);
    }

    private static string NormalizePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        // Step 1: Replace common path separators and spaces with normalized characters
        var normalized = path
            .Replace(" - ", "_")
            .Replace(" ", "_")
            .Replace("\\", ".")
            .Replace("/", ".");

        // Step 2: Remove diacritical marks (accents)
        // First, decompose characters into their base form and combining characters
        normalized = normalized.Normalize(NormalizationForm.FormD);

        // Remove all combining characters (accents, diacritics)
        normalized = new string(
            normalized.Where(character =>
                CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark
            ).ToArray()
        );

        // Step 3: Final cleanup
        // - Remove all remaining special characters except underscores and dots
        // - Convert to lowercase for consistency
        return NormalizeFullPathRegex().Replace(normalized, string.Empty).ToLowerInvariant();
    }

    public string Name { get; init; }

    public string FullName { get; init; }

    public string FullNameNormalized { get; private set; }

    public VideoFileContainer? Container { get; init; }

    public string? DirectoryPath { get; init; }

    public char DirectorySeparatorChar { get; init; }
}