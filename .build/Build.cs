using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Helpers;

partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory
                .GlobDirectories("**/bin", "**/obj")
                .ForEach(x => x.DeleteDirectory());
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            EnsureSolutionExists(AllSolutionFile);

            DotNetRestore(s => s
                .SetProjectFile(AllSolutionFile));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            if (!InvokedTargets.Contains(Restore))
                EnsureSolutionExists(AllSolutionFile);

            DotNetBuild(s => s
                .SetProjectFile(AllSolutionFile)
                .SetConfiguration(Configuration)
                .SetNoRestore(InvokedTargets.Contains(Restore)));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(AllSolutionFile)
                .SetConfiguration(Configuration)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetNoRestore(InvokedTargets.Contains(Restore)));
        });

    Target Reset => _ => _
        .Executes(() =>
        {
            TryDelete(AllSolutionFile);
            EnsureSolutionExists(AllSolutionFile);

            DotNetRestore(s => s
                .SetProjectFile(AllSolutionFile));
        });

    Target MatrixGenerate => _ => _
        .Executes(() =>
        {
            EnsureSolutionExists(AllSolutionFile);

            var solution = AllSolutionFile.ReadSolution();
            var matrix = new
            {
                include = solution
                    .GetAllProjects("*.Tests")
                    .Select(p => new
                    {
                        name = Path.GetFileNameWithoutExtension(p.Path),
                        path = RootDirectory.GetRelativePathTo(p.Path).ToString(),
                        directoryPath = Path.GetDirectoryName(p.Path)
                    })
                    .ToArray()
            };

            File.WriteAllText(RootDirectory / "matrix.json", JsonConvert.SerializeObject(matrix, Formatting.Indented));
        });
}
