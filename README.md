# Jellyfin Plugin: Ignore Empty Folders

Automatically hides TV shows that have no video files in their folders.

## What it does

After every library scan, the plugin finds all TV series with zero actual episodes and removes them from the Jellyfin database. The folders on disk are left untouched — when you later add video files, the next scan picks them up and the show stays.

This is useful if you maintain a folder structure for upcoming shows (with subtitles, NFO files, etc.) but don't want them cluttering your library until the actual video files are available.

## Installation

1. Download or build the plugin DLL
2. Copy it to your Jellyfin plugins directory:
   ```
   /config/plugins/Ignore Empty Folders_1.0.0.0/Jellyfin.Plugin.IgnoreEmptyFolders.dll
   ```
3. Restart Jellyfin

### Building from source

Requires .NET 9 SDK:

```bash
dotnet build -c Release
```

The DLL is output to `Jellyfin.Plugin.IgnoreEmptyFolders/bin/Release/net9.0/`.

## Configuration

Go to **Dashboard > Plugins > Ignore Empty Folders** to configure:

| Setting | Default | Description |
|---------|---------|-------------|
| Enable plugin | On | Master toggle. When disabled, no cleanup runs. |
| Log removed shows | On | Write a log entry for each series that gets removed. |

## How it works

The plugin provides two mechanisms:

1. **Post-scan task** — Runs automatically after every library scan. Finds all `Series` items with zero non-virtual, non-missing `Episode` children and deletes them from the database.

2. **Scheduled task** — "Clean Empty Series" appears in **Dashboard > Scheduled Tasks**. Runs every 24 hours by default. Can also be triggered manually.

Both mechanisms share the same logic and respect the plugin's configuration.

### What gets removed

A series is removed when it has **zero** episodes that are:
- Non-virtual (not metadata-only placeholders)
- Non-missing (not marked as unavailable)

Series with any real video files are always kept.

### What stays on disk

The plugin only removes entries from Jellyfin's database. Your folder structure, metadata files, subtitles, and any other files on disk are never touched.

## Limitations

- Empty series will briefly appear during a library scan before the post-scan task removes them
- If the plugin is disabled or uninstalled, empty series will reappear on the next scan
- The "An error occurred while getting the plugin details from the repository" message in the dashboard is normal for manually-installed plugins
