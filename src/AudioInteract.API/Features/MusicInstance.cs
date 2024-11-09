// <copyright file="MusicInstance.cs" company="Klybok Team">
// Copyright (c) Klybok Team. All rights reserved.
// </copyright>

namespace AudioInteract.Features;

using System.Linq;
using AudioInteract.API.Events.EventArgs;
using AudioInteract.Features.Events;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SCPSLAudioApi.AudioCore;
using UnityEngine;
using VoiceChat;

/// <summary>
/// Music Instance API for NPC. Main API to interact with SCPSLAudioPlayer.
/// </summary>
public class MusicInstance
{
    private LoggedType loggedType = LoggedType.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicInstance"/> class.
    /// </summary>
    /// <param name="linknpc">NPC to link.</param>
    public MusicInstance(Npc linknpc)
    {
        this.Npc = linknpc;

        this.AudioPlayerBase = AudioPlayerBase.Get(linknpc.ReferenceHub);

        if (!MusicAPI.IsEventsRegistered)
        {
            AudioPlayerBase.OnTrackSelected += (AudioPlayerBase playerBase, bool directPlay, int queuePos, ref string track) =>
            {
                TrackSelectedEventArgs ev = new(playerBase, track, directPlay, queuePos);

                Events.Track.OnTrackSelected(ev);
            };

            AudioPlayerBase.OnTrackLoaded += (AudioPlayerBase playerBase, bool directPlay, int queuePos, string track) =>
            {
                TrackLoadedEventArgs ev = new(playerBase, track, directPlay, queuePos);

                Events.Track.OnTrackLoaded(ev);
            };

            AudioPlayerBase.OnFinishedTrack += (AudioPlayerBase playerBase, string track, bool directPlay, ref int nextQueuePos) =>
            {
                TrackFinishedEventArgs ev = new(playerBase, track, directPlay, nextQueuePos);

                Events.Track.OnTrackFinished(ev);
            };

            Track.TrackFinished += OnFinished_ClearIfClearOnFinish;
            Exiled.Events.Handlers.Player.Destroying += OnDestroying_ClearProperly;

            MusicAPI.IsEventsRegistered = true;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicInstance"/> class.
    /// </summary>
    [Obsolete("Do not register classes like that. You class will be empty. Instead create class with NPC.", true)]
#pragma warning disable CS8618
    public MusicInstance()
    {
    }
#pragma warning restore CS8618

    /// <summary>
    /// Gets NPC linked to class.
    /// </summary>
    public Npc Npc { get; private set; }

    /// <summary>
    /// Gets or sets IDs who recive playing music.
    /// </summary>
    public List<int> PlaysFor
    {
        get => this.AudioPlayerBase.BroadcastTo;
        set => this.AudioPlayerBase.BroadcastTo = value;
    }

    /// <summary>
    /// Gets or sets players who recive playing music.
    /// </summary>
    public List<Player> PlaysForPlayers
    {
        get
        {
            List<Player> played = new();

            this.AudioPlayerBase.BroadcastTo.ForEach(x =>
            {
                if (Player.TryGet(x, out Player pl))
                {
                    played.Add(pl);
                }
            });

            return played;
        }

        set => this.AudioPlayerBase.BroadcastTo = value.Select(x => x.Id).ToList();
    }

    /// <summary>
    /// Gets <see cref="AudioPlayerBase"/> main class.
    /// </summary>
    public AudioPlayerBase AudioPlayerBase { get; private set; }

    /// <summary>
    /// Gets track queue. Doesn't show current playing track.
    /// </summary>
    public List<string> TrackQueue => this.AudioPlayerBase.AudioToPlay;

    /// <summary>
    /// Gets current playing track.
    /// </summary>
    public string PlayingTrack => this.AudioPlayerBase.CurrentPlay;

    /// <summary>
    /// Gets or sets a value indicating whether current track are looped or not.
    /// </summary>
    public bool IsLoop
    {
        get => this.AudioPlayerBase.Loop;
        set => this.AudioPlayerBase.Loop = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether playing tracks are mixed or not.
    /// </summary>
    public bool IsShuffle
    {
        get => this.AudioPlayerBase.Shuffle;
        set => this.AudioPlayerBase.Shuffle = value;
    }

    /// <summary>
    /// Gets or sets voice chat channel of bot.
    /// </summary>
    public VoiceChatChannel VoiceChatChannel
    {
        get => this.AudioPlayerBase.BroadcastChannel;
        set => this.AudioPlayerBase.BroadcastChannel = value;
    }

    /// <summary>
    /// Gets a value indicating whether track is finished or not.
    /// </summary>
    public bool IsFinished => this.AudioPlayerBase.IsFinished
        || (this.AudioPlayerBase.VorbisReader != null && this.AudioPlayerBase.VorbisReader.Streams != null && !this.AudioPlayerBase.VorbisReader.Streams.Any());

    /// <summary>
    /// Gets or sets logs type <see cref="Features.LoggedType"/>.
    /// </summary>
    public LoggedType LoggedType
    {
        get => this.loggedType;
        set
        {
            this.AudioPlayerBase.LogDebug = false;
            this.AudioPlayerBase.LogInfo = false;

            switch (value)
            {
                case LoggedType.Info:
                    this.AudioPlayerBase.LogInfo = true;
                    break;
                case LoggedType.Debug:
                    this.AudioPlayerBase.LogDebug = true;
                    this.AudioPlayerBase.LogInfo = true;
                    break;
                default:
                    break;
            }

            this.loggedType = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether remove NPC on finish or not.
    /// </summary>
    public bool ClearOnFinish { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether shuffle tracks or not.
    /// </summary>
    public bool Shuffle
    {
        get => this.AudioPlayerBase.Shuffle;
        set => this.AudioPlayerBase.Shuffle = value;
    }

    /// <summary>
    /// Gets or sets volume of bot.
    /// </summary>
    public float Volume
    {
        get => this.AudioPlayerBase.Volume;
        set => this.AudioPlayerBase.Volume = value;
    }

    /// <summary>
    /// Destroy NPC properly.
    /// </summary>
    /// <param name="ev">Event.</param>
    public static void OnDestroying_ClearProperly(DestroyingEventArgs ev)
    {
        foreach (MusicInstance? item in MusicAPI.MusicInstances.Where(x => x.Npc == ev.Player).ToList())
        {
            MusicAPI.DestroyNPC(item);
        }
    }

    /// <summary>
    /// If clear on finish, if finished clear (lmao).
    /// </summary>
    /// <param name="ev">Event.</param>
    public static void OnFinished_ClearIfClearOnFinish(TrackFinishedEventArgs ev)
    {
        if (ev.MusicInstance == null || !ev.MusicInstance.ClearOnFinish)
        {
            return;
        }

        Timing.CallDelayed(0.5f, () =>
        {
            if (ev.MusicInstance.IsFinished)
            {
                MusicAPI.DestroyNPC(ev.MusicInstance);
            }
        });
    }

    /// <summary>
    /// Immediately starts playing music.
    /// </summary>
    /// <param name="audioFile">Plays the <see cref="AudioFile"/>.</param>
    /// <returns>Indicates music is launched or not.</returns>
    public bool Play(AudioFile audioFile)
    {
        if (!audioFile.IsEnabled)
        {
            Log.Warn("AudioFile is not enabled, skipping.");
            return false;
        }

        if (!Extensions.CheckTrack(audioFile))
        {
            Log.Error($"Audio file extention doesn't support. ({audioFile.FilePath})");
            return false;
        }

        Timing.CallDelayed(0.2f, () =>
        {
            this.Npc!.Role.Set(audioFile.RoleType, RoleSpawnFlags.UseSpawnpoint);

            this.Npc!.Scale = audioFile.Scale;

            if (this.Npc.Role is FpcRole fpcRole)
            {
                fpcRole.IsNoclipEnabled = audioFile.IsNoclipEnabled;
            }

            if (audioFile.RoomType != RoomType.Unknown && audioFile.LocalRoomPostion != Vector3.zero)
            {
                this.Npc!.Position = Room.Get(audioFile.RoomType).WorldPosition(audioFile.LocalRoomPostion) + Vector3.up;
            }

            if (audioFile.Postion != Vector3.zero)
            {
                this.Npc!.Position = audioFile.Postion;
            }
        });

        this.AudioPlayerBase.BroadcastChannel = audioFile.VoiceChannel;
        this.AudioPlayerBase.Volume = audioFile.Volume;
        this.AudioPlayerBase.Loop = audioFile.IsLooped;

        try
        {
            this.AudioPlayerBase.Enqueue(audioFile.FilePath, 0);

            this.AudioPlayerBase.Play(0);

            this.AudioPlayerBase.IsFinished = false;
        }
        catch (System.Exception ex)
        {
            Log.Error(ex);
        }

        return true;
    }

    // bro tf is this summary lol

    /// <summary>
    /// Check track.
    /// </summary>
    /// <param name="audioFile">Audio File to check.</param>
    /// <returns>Is track is normal or not.</returns>
    public bool CheckTrack(AudioFile audioFile)
    {
        return Extensions.CheckTrack(audioFile);
    }

    /// <summary>
    /// Immediately starts playing music. API also supports links.
    /// </summary>
    /// <param name="path">Path to file.</param>
    /// <param name="voiceChannel">Voice channel to play.</param>
    /// <param name="loop">Loop track or not.</param>
    /// <returns>Indicates music is launched or not.</returns>
    public bool Play(string path, VoiceChatChannel voiceChannel = VoiceChatChannel.Intercom, bool loop = false)
    {
        this.AudioPlayerBase.BroadcastChannel = voiceChannel;

        this.AudioPlayerBase.Loop = loop;

        this.AudioPlayerBase.Enqueue(path, 0);

        if (this.AudioPlayerBase.CurrentPlay == null || this.AudioPlayerBase.IsFinished)
        {
            this.AudioPlayerBase.Play(0);
            this.AudioPlayerBase.IsFinished = false;
        }

        return true;
    }

    /// <summary>
    /// Enqueue music.
    /// </summary>
    /// <param name="audioFile">Enqueue the <see cref="AudioFile"/>.</param>
    public void Enqueue(AudioFile audioFile)
    {
        this.AudioPlayerBase.Enqueue(audioFile.FilePath, -1);

        // If music not playing, starts to play.
        if (this.AudioPlayerBase.CurrentPlay == null || this.AudioPlayerBase.IsFinished)
        {
            this.AudioPlayerBase.Play(0);
            this.AudioPlayerBase.IsFinished = false;
        }
    }

    /// <summary>
    /// Enqueue music.
    /// </summary>
    /// <param name="path">Path to file.</param>
    /// <returns>Is music started or not.</returns>
    public bool Enqueue(string path)
    {
        this.AudioPlayerBase.Enqueue(path, -1);

        // If music not playing, starts to play.
        if (this.AudioPlayerBase.CurrentPlay == null || this.AudioPlayerBase.IsFinished)
        {
            this.AudioPlayerBase.Play(0);
            this.AudioPlayerBase.IsFinished = false;
        }

        return true;
    }

    /// <summary>
    /// Start playing music if current play is null and there is tracks in queue.
    /// </summary>
    public void Play()
    {
        if (this.PlayingTrack == null && this.TrackQueue.Count > 0)
        {
            this.AudioPlayerBase.Play(0);
            this.AudioPlayerBase.IsFinished = false;
        }
    }

    /// <summary>
    /// Skip current playing track.
    /// </summary>
    public void Skip() => this.AudioPlayerBase.Stoptrack(false);

    /// <summary>
    /// Stop playing music. Remove all music in queue.
    /// </summary>
    public void Stop()
    {
        this.AudioPlayerBase.Stoptrack(true);

        this.AudioPlayerBase.CurrentPlay = null;
        this.AudioPlayerBase.IsFinished = true;
    }
}