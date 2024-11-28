// <copyright file="EventHandlers.cs" company="Klybok Team">
// Copyright (c) Klybok Team. All rights reserved.
// </copyright>

namespace AudioInteract.Plugin.Handlers;

using AudioInteract.API.Features;
using AudioInteract.Features;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Warhead;
using ServerEvent = Exiled.Events.Handlers.Server;
using WarheadEvent = Exiled.Events.Handlers.Warhead;

/// <summary/>
public class EventHandlers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlers"/> class.
    /// </summary>
    public EventHandlers()
    {
        if (!PluginInstance.Instance!.Config.IsEventsEnabled)
        {
            return;
        }

        ServerEvent.WaitingForPlayers += this.OnWaitingForPlayers_EnableMusic;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="EventHandlers"/> class.
    /// </summary>
    ~EventHandlers()
    {
        if (!PluginInstance.Instance!.Config.IsEventsEnabled)
        {
            return;
        }

        ServerEvent.WaitingForPlayers -= this.OnWaitingForPlayers_EnableMusic;
    }

    /// <summary>
    /// Gets lobby playing NPCs.
    /// </summary>
    public static Speaker LobbyPlayingSpeaker { get; private set; } = new();

    /// <summary>
    /// Plays music (if exists) on waiting for players (lobby).
    /// </summary>
    public void OnWaitingForPlayers_EnableMusic()
    {
        foreach (AudioFile audioFile in PluginInstance.Instance!.Config.LobbyMusic.Where(x => x.IsEnabled))
        {
            Log.Info("sirnie sasikski");
            Speaker? speaker = new();

            speaker.Play(audioFile.FilePath);

            speaker.IsSpatial = true;
        }
    }
}
