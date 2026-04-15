using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.IgnoreEmptyFolders.Tasks;

public class CleanEmptySeriesPostScanTask : ILibraryPostScanTask
{
    private readonly ILibraryManager _libraryManager;
    private readonly ILogger<CleanEmptySeriesPostScanTask> _logger;

    public CleanEmptySeriesPostScanTask(
        ILibraryManager libraryManager,
        ILogger<CleanEmptySeriesPostScanTask> logger)
    {
        _libraryManager = libraryManager;
        _logger = logger;
    }

    public Task Run(IProgress<double> progress, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var cleaner = new EmptySeriesCleaner(_libraryManager, _logger);
            cleaner.CleanEmptySeries(progress, cancellationToken);
        }, cancellationToken);
    }
}
