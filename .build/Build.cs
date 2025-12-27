using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    private readonly Configuration _configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            Helpers.DotNetBuildSolution(AllSolutionFile);
            DotNetRestore(x => x.SetProjectFile(AllSolutionFile));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            if (!InvokedTargets.Contains(Restore))
                Helpers.DotNetBuildSolution(AllSolutionFile);

            DotNetBuild(c => c
                .SetProjectFile(AllSolutionFile)
                .SetNoRestore(InvokedTargets.Contains(Restore))
                .SetConfiguration(_configuration));
        });

    Target MatrixGenerate => _ => _
        .Executes(() =>
        {
            Helpers.DotNetBuildSolution(AllSolutionFile);
            var all = AllSolutionFile.ReadSolution();

            var matrix = new
            {
                include = all
                    .GetAllProjects("*.Tests")
                    .Select(p => new
                    {
                        name = Path.GetFileNameWithoutExtension(p.Path),
                        path = Path.GetRelativePath(RootDirectory, p.Path),
                        directoryPath = Path.GetDirectoryName(p.Path)
                    }).ToArray()
            };

            File.WriteAllText(RootDirectory / "matrix.json", JsonConvert.SerializeObject(matrix));
        });
}
