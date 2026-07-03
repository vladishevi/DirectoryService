namespace DirectoryService.IntegrationTests.BackgroundServices;

public sealed record CleanupOptions
{
    public TimeSpan Interval { get; init; }
    public TimeSpan RetentionPeriod  { get; init; }
}