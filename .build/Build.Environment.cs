using Nuke.Common.IO;

partial class Build
{
    AbsolutePath SourceDirectory => RootDirectory / "src";

    AbsolutePath AllSolutionFile => SourceDirectory / "All.slnx";
}
