// <copyright file="EventHandlers.cs" company="Klybok Team">
// Copyright (c) Klybok Team. All rights reserved.
// </copyright>

namespace AudioInteract.Plugin.Handlers;

using AudioInteract.Features;
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
        ServerEvent.RoundStarted += this.OnRoundStarted_StopMusic;
        ServerEvent.RespawningTeam += this.OnRespawningTeam_PlayMusic;

        WarheadEvent.Starting += this.OnWarheadStarted_PlayMusic;
        WarheadEvent.Detonated += this.OnWarheadDetonated_StopMusic;
        WarheadEvent.Stopping += this.OnWarheadStopping_StopMusic;
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
        ServerEvent.RoundStarted -= this.OnRoundStarted_StopMusic;
        ServerEvent.RespawningTeam -= this.OnRespawningTeam_PlayMusic;

        WarheadEvent.Starting -= this.OnWarheadStarted_PlayMusic;
        WarheadEvent.Detonated -= this.OnWarheadDetonated_StopMusic;
        WarheadEvent.Stopping -= this.OnWarheadStopping_StopMusic;
    }

    /// <summary>
    /// Gets lobby playing NPCs.
    /// </summary>
    public static List<MusicInstance_Old> LobbyPlayingNPC { get; private set; } = new();

    /// <summary>
    /// Gets warhead NPC playing.
    /// </summary>
    public static MusicInstance_Old? WarheadNPC { get; private set; }

    /// <summary>
    /// Plays music after team respawn.
    /// </summary>
    /// <param name="ev">Event.</param>
    public void OnRespawningTeam_PlayMusic(RespawningTeamEventArgs ev)
    {
        if (!ev.IsAllowed
            || PluginInstance.Instance == null)
        {
            return;
        }

        PluginInstance.Instance.Config.TeamMusic.TryGetValue(ev.NextKnownTeam, out AudioFile? teamAudio);

        if (teamAudio == null || !teamAudio.IsEnabled)
        {
            return;
        }

        MusicInstance_Old npc = MusicAPI.CreateNPC(teamAudio.BotName);

        npc.Play(teamAudio);

        npc.ClearOnFinish = true;
    }

    /// <summary>
    /// Plays music (if exists) on waiting for players (lobby).
    /// </summary>
    public void OnWaitingForPlayers_EnableMusic()
    {
        foreach (AudioFile audioFile in PluginInstance.Instance!.Config.LobbyMusic.Where(x => x.IsEnabled))
        {
            MusicInstance_Old? musicInstance = MusicAPI.CreateNPC(audioFile.BotName);

            if (musicInstance == null)
            {
                continue;
            }

            musicInstance.LoggedType = LoggedType.Info;

            musicInstance.Play(audioFile);

            LobbyPlayingNPC.Add(musicInstance);
        }
    }

    /// <summary>
    /// Stops music if was playing in lobby.
    /// </summary>
    public void OnRoundStarted_StopMusic()
    {
        foreach (MusicInstance_Old musicInstance in LobbyPlayingNPC)
        {
            MusicAPI.DestroyNPC(musicInstance);
        }
    }

    /// <summary>
    /// Stops warhead music on detonated event.
    /// </summary>
    public void OnWarheadDetonated_StopMusic() => this.Warhead_StopMusic();

    /// <summary>
    /// Stops warhead music on stopping event.
    /// </summary>
    /// <param name="ev">Event.</param>
    public void OnWarheadStopping_StopMusic(StoppingEventArgs ev) => this.Warhead_StopMusic();

    /// <summary>
    /// Stops warhead music.
    /// </summary>
    public void Warhead_StopMusic()
    {
        if (WarheadNPC != null)
        {
            MusicAPI.DestroyNPC(WarheadNPC);
        }
    }

    /// <summary>
    /// Plays music (if exists) on warhead start.
    /// </summary>
    /// <param name="ev">Event.</param>
    public void OnWarheadStarted_PlayMusic(StartingEventArgs ev)
    {
        if (!ev.IsAllowed
            || PluginInstance.Instance == null)
        {
            return;
        }

        AudioFile warheadAudio = PluginInstance.Instance.Config.WarheadMusic;

        if (warheadAudio == null || !warheadAudio.IsEnabled)
        {
            return;
        }

        if (WarheadNPC != null)
        {
            MusicAPI.DestroyNPC(WarheadNPC);
        }

        WarheadNPC = MusicAPI.CreateNPC(warheadAudio.BotName);

        WarheadNPC.Play(warheadAudio);
    }
}
