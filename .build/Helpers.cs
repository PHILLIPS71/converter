using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

static class Helpers
{
    static readonly string[] Directories =
    [
    ];

    static IEnumerable<string> GetAllProjects(string source, IEnumerable<string> directories)
    {
        return directories
            .Select(directory => Path.Combine(source, directory))
            .SelectMany(path => Directory.EnumerateFiles(path, "*.csproj", SearchOption.AllDirectories));
    }

    public static IReadOnlyCollection<Output> DotNetBuildSolution(string solution)
    {
        if (File.Exists(solution))
            return Array.Empty<Output>();

        var root = Path.GetDirectoryName(solution);
        if (string.IsNullOrWhiteSpace(root))
            return Array.Empty<Output>();

        var projects = GetAllProjects(root, Directories);
        var working = Path.GetDirectoryName(solution);

        var list = new List<Output>();

        var args = string.Join(" ", projects.Select(t => $"\"{t}\""));
        list.AddRange(DotNetTasks.DotNet($"new sln -n {Path.GetFileNameWithoutExtension(solution)}", working));
        list.AddRange(DotNetTasks.DotNet($"sln \"{solution}\" add {args}", working));

        return list;
    }
}