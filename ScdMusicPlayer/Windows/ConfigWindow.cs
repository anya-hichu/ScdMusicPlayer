using System;
using System.Numerics;
using Dalamud.Interface.Colors;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace ScdMusicPlayer.Windows;

public class ConfigWindow : Window, IDisposable
{
    private const int MaxPathSize = 256;
    private readonly Vector2 HorizontalSpacing = new Vector2(0, ImGui.GetTextLineHeightWithSpacing());

    private Configuration Configuration;
    private FileDialogManager FileDialogManager;

    public ConfigWindow(Plugin plugin) : base("SCD Music Player Config##ScdMusicPlayerConfig")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(400, 320);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;
        FileDialogManager = new FileDialogManager();
    }

    public void Dispose() {}

    public override void Draw()
    {
        if (ImGui.CollapsingHeader("Browser", ImGuiTreeNodeFlags.DefaultOpen))
        {
            var browserDefaultPath = Configuration.BrowserDefaultPath;
            ImGui.Text("Default folder*:");
            ImGui.SameLine(90);
            ImGui.SetNextItemWidth(250);
            if (ImGui.InputText("##browserDefaultPathInput", ref browserDefaultPath, MaxPathSize))
            {
                Configuration.BrowserDefaultPath = browserDefaultPath;
                Configuration.Save();
            }
            ImGui.SameLine();
            if (ImGui.Button("Select##browserDefaultPathSelect"))
            {
                FileDialogManager.SaveFolderDialog("Default Folder Path", Configuration.BrowserDefaultPath, (valid, selectedPath) =>
                {
                    Configuration.BrowserDefaultPath = selectedPath;
                    Configuration.Save();
                });
            }
            ImGui.Dummy(HorizontalSpacing);
        }

        if (ImGui.CollapsingHeader("SCD Exporter", ImGuiTreeNodeFlags.DefaultOpen))
        {
            var exporterScdPath = Configuration.ExporterScdPath;
            ImGui.Text("File location*:");
            ImGui.SameLine(90);
            ImGui.SetNextItemWidth(250);
            if (ImGui.InputText("##exporterScdPathInput", ref exporterScdPath, MaxPathSize))
            {
                Configuration.ExporterScdPath = exporterScdPath;
                Configuration.Save();
            }
            ImGui.SameLine();
            if (ImGui.Button("Select##exportedFileSelect"))
            {
                FileDialogManager.SaveFileDialog("Export location*", "{.scd}", Configuration.ExporterScdPath, ".scd", (valid, selectedPath) =>
                {
                    if (valid)
                    {
                        Configuration.ExporterScdPath = selectedPath;
                        Configuration.Save();
                    }
                });
            }
            ImGui.Dummy(HorizontalSpacing);
        }

        if (!Configuration.isValid())
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, "Invalid configuration, please verify");
        }

        FileDialogManager.Draw();
    }
}
