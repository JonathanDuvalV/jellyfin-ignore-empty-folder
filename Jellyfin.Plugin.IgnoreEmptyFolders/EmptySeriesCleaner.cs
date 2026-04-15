using Jellyfin.Data.Enums;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Querying;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.IgnoreEmptyFolders;

public class EmptySeriesCleaner
{
    private readonly ILibraryManager _libraryManager;
    private readonly ILogger _logger;

    public EmptySeriesCleaner(ILibraryManager libraryManager, ILogger logger)
    {
        _libraryManager = libraryManager;
        _logger = logger;
    }

    public int CleanEmptySeries(IProgress<double>? progress, CancellationToken cancellationToken)
    {
        var config = Plugin.Instance?.Configuration;
        if (config is not null && !config.Enabled)
        {
            _logger.LogInformation("Ignore Empty Folders: Plugin is disabled, skipping");
            return 0;
        }

        var logDeletions = config?.LogDeletions ?? true;

        var seriesList = _libraryManager.GetItemList(new InternalItemsQuery
        {
            IncludeItemTypes = new[] { BaseItemKind.Series },
            DtoOptions = new DtoOptions(false) { EnableImages = false }
        });

        _logger.LogInformation("Ignore Empty Folders: Checking {Count} series for empty folders", seriesList.Count);

        var removedCount = 0;
        var total = seriesList.Count;

        for (var i = 0; i < total; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (seriesList[i] is not Series series)
            {
                continue;
            }

            var seriesKey = series.GetPresentationUniqueKey();

            var episodeCount = _libraryManager.GetCount(new InternalItemsQuery
            {
                AncestorWithPresentationUniqueKey = null,
                SeriesPresentationUniqueKey = seriesKey,
                IncludeItemTypes = new[] { BaseItemKind.Episode },
                IsVirtualItem = false,
                IsMissing = false,
                Limit = 0,
                DtoOptions = new DtoOptions(false) { EnableImages = false }
            });

            if (episodeCount > 0)
            {
                continue;
            }

            if (logDeletions)
            {
                _logger.LogInformation(
                    "Ignore Empty Folders: Removing series \"{SeriesName}\" - no video files found",
                    series.Name);
            }

            try
            {
                _libraryManager.DeleteItem(
                    series,
                    new DeleteOptions { DeleteFileLocation = false });

                removedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ignore Empty Folders: Failed to remove series \"{SeriesName}\"", series.Name);
            }
            progress?.Report((double)(i + 1) / total * 100);
        }

        _logger.LogInformation("Ignore Empty Folders: Removed {Count} empty series", removedCount);
        return removedCount;
    }
}
