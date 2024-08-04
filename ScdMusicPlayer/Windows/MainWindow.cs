using System;
using System.IO;
using System.Numerics;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;

namespace ScdMusicPlayer.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin Plugin;
    private ICommandManager CommandManager;
    private FileDialogManager FileDialogManager;
    private String BlankScdPath;

    private State PlayerState;

    public MainWindow(Plugin plugin, ICommandManager commandManager, String blankScdPath)
        : base("SCD Music Player##ScdMusicPlayer", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        Plugin = plugin;
        CommandManager = commandManager;
        BlankScdPath = blankScdPath;
        FileDialogManager = new FileDialogManager();
        PlayerState = new State();

        PlayerState.OnChanges += OnStateChanges;
    }

    public void Dispose() { 
        PlayerState.OnChanges -= OnStateChanges;
    }

    public override void Draw()
    {
        if (Plugin.Configuration.isValid())
        {
            if (PlayerState.HasTrack())
            {
                if (ImGui.CollapsingHeader("Controls", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    var state = PlayerState;
                    if (ImGui.Button("Previous##previousControl"))
                    {
                        state.PreviousTrack();
                    }

                    ImGui.SameLine();

                    if (!state.IsPlaying)
                    {
                        if (ImGui.Button($"Play##playControl"))
                        {
                            state.PlayTrack();
                        }
                        ImGui.SameLine();
                    }

                    if (state.IsPlaying)
                    {
                        if (ImGui.Button($"Stop##stopControl"))
                        {
                            state.StopTrack();
                        }
                        ImGui.SameLine();
                    }

                    if (ImGui.Button($"Next##nextControl"))
                    {
                        state.NextTrack();
                    }
                }
            }

            if (ImGui.CollapsingHeader("Playlist", ImGuiTreeNodeFlags.DefaultOpen))
            {
                if(ImGui.Button("Add tracks##addTracks"))
                {
                    // Add other formats later with converters
                    FileDialogManager.OpenFileDialog("Add tracks to playlist", "{.scd}", (valid, paths) =>
                    {
                        if (valid)
                        {
                            PlayerState.AddTracks(paths);
                        }
                    }, 0, Plugin.Configuration.BrowserDefaultPath);
                }

                ImGui.SameLine();
                if (ImGui.Button("Remove all##removeAll"))
                {
                    PlayerState.RemoveAllTracks();
                }

                if (ImGui.BeginTable("playlistTable", 4, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY, new Vector2(ImGui.GetWindowWidth(), ImGui.GetWindowHeight() - 150f)))
                {
                    ImGui.TableSetupColumn(String.Empty, ImGuiTableColumnFlags.None, 1);
                    ImGui.TableSetupColumn("N°", ImGuiTableColumnFlags.None, 1);
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.None, 10);
                    ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.None, 3);
                    ImGui.TableHeadersRow();

                    PlayerState.forEachTrack((i, path) =>
                    {
                        if (ImGui.TableNextColumn())
                        {
                            if (PlayerState.Index == i)
                            {
                                ImGui.Text(PlayerState.IsPlaying? "Playing" : "Paused");          
                            }
                            
                        }

                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text($"#{i + 1}");
                        }

                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(Path.GetFileName(path));
                        }

                        if (ImGui.TableNextColumn())
                        {
                            if (ImGui.Button($"Play###playTrack{i}"))
                            {
                                PlayerState.PlayTrack(i);
                            }

                            ImGui.SameLine(40);
                            if (PlayerState.CanMoveTrackUp(i))
                            {
                                if (ImGui.Button($"↑###moveTrackUp{i}"))
                                {
                                    PlayerState.MoveTrackUp(i);
                                }
                            }

                            if (PlayerState.CanMoveTrackDown(i))
                            {
                                ImGui.SameLine(65);
                                if (ImGui.Button($"↓###moveTrackDown{i}"))
                                {
                                    PlayerState.MoveTrackDown(i);
                                }
                            }

                            ImGui.SameLine(92);

                            if (ImGui.Button($"X###removeTrack{i}"))
                            {
                                PlayerState.RemoveTrack(i);
                            }
                        }
                        ImGui.TableNextRow();
                    });

                    ImGui.EndTable();
                }
            }
        } 
        else
        {
            ImGui.Text("Please configure plugin first");
            if(ImGui.Button("Settings"))
            {
                Plugin.ToggleConfigUI();
            }
        }

        FileDialogManager.Draw();
    }

    private void OnStateChanges()
    {
        if (PlayerState.IsPlaying)
        {
            ExportTrack(PlayerState.TrackPath());
        }
        else
        {
            ExportTrack(BlankScdPath);
        }
        RedrawSelf();
    }

    private void ExportTrack(string path)
    {
        File.Copy(path, Plugin.Configuration.ExporterScdPath, true);
    }

    private void RedrawSelf()
    {
        if (!CommandManager.ProcessCommand("/penumbra redraw <me>"))
        {
            // Log error if penumbra redraw fail
        }
    }
}
