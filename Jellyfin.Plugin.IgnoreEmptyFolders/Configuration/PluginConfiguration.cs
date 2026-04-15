using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.IgnoreEmptyFolders.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public bool Enabled { get; set; } = true;

    public bool LogDeletions { get; set; } = true;
}
