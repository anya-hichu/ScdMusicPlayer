using System;
using System.Collections.Generic;

namespace ScdMusicPlayer;

public class State
{
    private List<string> Playlist = [];
    public bool IsPlaying { get; private set; } = false;
    public int Index { get; private set; } = 0;

    public event Action? OnChanges;

    public bool HasTrack()
    {
        return Playlist.Count > 0;
    }

    public string TrackPath()
    {
        return Playlist[Index];
    }

    public void AddTracks(List<string> tracks)
    {
        Playlist.AddRange(tracks);
    }

    public void RemoveAllTracks()
    { 
        Playlist.Clear();
        Index = 0;
        IsPlaying = false;
        Notify();
    }

    public void forEachTrack(Action<int, string> action)
    {
        for (var i = 0; i < Playlist.Count; i++)
        {
            action(i, Playlist[i]);
        }
    }

    public void PlayTrack(int i)
    {
        Index = i;
        IsPlaying = true;
        Notify();
    }

    public bool CanMoveTrackUp(int i)
    {
        return i > 0;
    }

    public void MoveTrackUp(int i)
    {
        var path = Playlist[i];
        Playlist.RemoveAt(i);
        Playlist.Insert(i - 1, path);

        if (Index == i)
        {
            Index = i - 1;
        }
    }

    public bool CanMoveTrackDown(int i)
    {
        return i < Playlist.Count - 1;
    }

    public void MoveTrackDown(int i)
    {
        var path = Playlist[i];
        Playlist.RemoveAt(i);
        Playlist.Insert(i + 1, path);

        if (Index == i)
        {
            Index = i + 1;
        }
    }

    public void RemoveTrack(int i)
    {
        Playlist.RemoveAt(i);
        if (Index == i)
        {
            if (Index >= Playlist.Count)
            {
                Index = 0;
                IsPlaying = false;
            }

            Notify();
        }
    }

    public void PreviousTrack()
    {
        if (Index > 0)
        {
            Index--;
        } 
        else
        {
            Index = Playlist.Count - 1;
        }

        IsPlaying = true;
        Notify();
    }

    public void PlayTrack()
    {
        IsPlaying = true;
        Notify();
    }

    public void StopTrack()
    {
        IsPlaying = false;
        Notify();
    }

    public void NextTrack()
    {
        if (Index < Playlist.Count - 1)
        {
            Index++;
        } 
        else
        {
            Index = 0;
        }
        IsPlaying = true;
        Notify();
    }

    private void Notify()
    {
        OnChanges?.Invoke();
    }
}
