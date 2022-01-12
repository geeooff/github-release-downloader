using System.CommandLine;
using GitHubReleaseDownloader;

// command options
//

var ownerOption = new Option<string>("--owner", "Owner Name")
{
    IsRequired = true
};

var repoOption = new Option<string>("--repo", "Repository Name")
{
    IsRequired = true
};

var outputOption = new Option<FileInfo>("--output", "Release asset destination file")
{
    IsRequired = true
};

var forceOption = new Option<bool>("--force", () => false, "Force download even if output is up to date")
{
    IsRequired = false
};

var preReleaseOption = new Option<bool>("--pre-release", () => false, "Allow Pre-Release asset downloads")
{
    IsRequired = false
};

var createBackupOption = new Option<bool>("--create-backup", () => true, "Create a backup of any existing file")
{
    IsRequired = false
};

// root command
//

var command = new RootCommand("GitHub Release Asset Downloader")
{
    ownerOption,
    repoOption,
    outputOption,
    forceOption,
    preReleaseOption,
    createBackupOption
};

// root command handler
//

command.SetHandler((context) => ProgramHandler.Execute(
    context.ParseResult.GetValueForOption(ownerOption)!,
    context.ParseResult.GetValueForOption(repoOption)!,
    context.ParseResult.GetValueForOption(outputOption)!,
    context.ParseResult.GetValueForOption(forceOption),
    context.ParseResult.GetValueForOption(preReleaseOption),
    context.ParseResult.GetValueForOption(createBackupOption),
    context.GetCancellationToken()
));

return await command.InvokeAsync(args);