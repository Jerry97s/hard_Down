namespace HdLabs.Finder.Models;

public sealed class FileSearchResult
{
    public required string Name { get; init; }
    public required string FullPath { get; init; }
    public required string DirectoryPath { get; init; }
    public long? SizeBytes { get; init; }
    public DateTimeOffset? LastWriteTime { get; init; }
}
