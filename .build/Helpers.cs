using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;

static class Helpers
{
    static readonly string[] Directories =
    [
        "Infrastructure",
        "Service.Runner",
        "Service.Supervisor"
    ];

    public static void EnsureSolutionExists(AbsolutePath path)
    {
        if (File.Exists(path))
            return;

        var solution = Path.GetFileNameWithoutExtension(path);

        DotNetTasks.DotNet($"new sln -n {solution} --format slnx", path.Parent);

        foreach (var (folder, projects) in GetProjectsByFolder(path.Parent))
        {
            var args = string.Join(" ", projects.Select(p => $"\"{p}\""));
            DotNetTasks.DotNet($"sln \"{path}\" add --solution-folder \"{folder}\" {args}", path.Parent);
        }
    }

    public static void TryDelete(AbsolutePath file)
    {
        if (File.Exists(file))
            File.Delete(file);
    }

    static IEnumerable<(string Folder, List<string> Projects)> GetProjectsByFolder(AbsolutePath path)
    {
        return Directories
            .Select(directory => path / directory)
            .Where(path => Directory.Exists(path))
            .SelectMany(directory => Directory.EnumerateFiles(directory, "*.csproj", SearchOption.AllDirectories))
            .GroupBy(project => GetSolutionFolder(path, project))
            .Select(group => (group.Key, group.ToList()));
    }

    static string GetSolutionFolder(AbsolutePath path, string project)
    {
        var relative = Path.GetRelativePath(path, project);
        var parts = relative.Replace('\\', '/').Split('/');

        // structure: TopLevelDir/Category/ProjectFolder/Project.csproj
        // returns: TopLevelDir/Category (e.g., "Infrastructure/src")
        return parts.Length >= 2 ? $"{parts[0]}/{parts[1]}" : parts[0];
    }
}
