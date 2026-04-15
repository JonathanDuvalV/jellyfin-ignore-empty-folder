using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.IgnoreEmptyFolders.Tasks;

public class CleanEmptySeriesScheduledTask : IScheduledTask
{
    private readonly ILibraryManager _libraryManager;
    private readonly ILogger<CleanEmptySeriesScheduledTask> _logger;

    public CleanEmptySeriesScheduledTask(
        ILibraryManager libraryManager,
        ILogger<CleanEmptySeriesScheduledTask> logger)
    {
        _libraryManager = libraryManager;
        _logger = logger;
    }

    public string Name => "Clean Empty Series";

    public string Key => "IgnoreEmptyFoldersCleanEmptySeries";

    public string Description => "Removes TV shows that have no video files from the library.";

    public string Category => "Library";

    public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var cleaner = new EmptySeriesCleaner(_libraryManager, _logger);
            cleaner.CleanEmptySeries(progress, cancellationToken);
        }, cancellationToken);
    }

    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
    {
        yield return new TaskTriggerInfo
        {
            Type = TaskTriggerInfoType.IntervalTrigger,
            IntervalTicks = TimeSpan.FromHours(24).Ticks
        };
    }
}
