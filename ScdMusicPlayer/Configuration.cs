using Dalamud.Configuration;
using System;
using System.IO;
namespace ScdMusicPlayer;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public String BrowserDefaultPath { get; set; } = String.Empty;
    public String ExporterScdPath { get; set; } = String.Empty;

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }

    public bool isValid()
    {
        return Directory.Exists(BrowserDefaultPath) && Directory.Exists(Path.GetDirectoryName(ExporterScdPath));
    }
}
