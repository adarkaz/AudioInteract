// <copyright file="Speaker.cs" company="Klybok Team">
// Copyright (c) Klybok Team. All rights reserved.
// </copyright>

namespace AudioInteract.API.Features;

using AdminToys;
using Exiled.API.Interfaces;
using MEC;
using NAudio.Lame;
using NAudio.Wave;
using VoiceChat;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;
using VoiceChat.Networking;
using VoiceChat.Playbacks;

/// <summary>
/// Wrapper for SpeakerToy.
/// </summary>
public class Speaker : IWrapper<SpeakerToy>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Speaker"/> class.
    /// </summary>
    public Speaker()
    {
        this.Base = new();

        Instances.Add(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Speaker"/> class.
    /// </summary>
    /// <param name="speakerToy"><see cref="SpeakerToy"/>.</param>
    public Speaker(SpeakerToy speakerToy)
    {
        this.Base = new();

        Instances.Add(this);
    }

    /// <summary>
    /// Gets all instances <see cref="Speaker"/>.
    /// </summary>
    public static List<Speaker> Instances { get; internal set; } = new();

    /// <summary>
    /// Gets <see cref="SpeakerToy"/>.
    /// </summary>
    public SpeakerToy Base { get; private set; }

    #region Fields

    /// <summary>
    /// Gets or sets a value indicating whether shuffle tracks or not.
    /// </summary>
    public bool Shuffle { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether track is looped or not.
    /// </summary>
    public bool Loop { get; set; } = false;

    /// <summary>
    /// Gets or sets volume of the speaker. This value is clamped between 0 and 1.
    /// </summary>
    public float Volume
    {
        get => this.Base.Volume;
        set => this.Base.Volume = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether audio should be played globally or in 3D space around the speaker toy (like intercom or pre-game chat).
    /// </summary>
    public bool IsSpatial
    {
        get => this.Base.IsSpatial;
        set => this.Base.IsSpatial = value;
    }

    /// <summary>
    /// Gets or sets the minimum distance at which the maximum volume level is audible.
    /// </summary>
    public float MinimumDistance
    {
        get => this.Base.MinDistance;
        set => this.Base.MinDistance = value;
    }

    /// <summary>
    /// Gets or sets the maximum distance at which audio source is audible.
    /// </summary>
    public float MaximumDistance
    {
        get => this.Base.MaxDistance;
        set => this.Base.MaxDistance = value;
    }

    #endregion

    #region Playing

    /// <summary>
    /// Gets or sets playback coroutine.
    /// </summary>
    public CoroutineHandle? PlaybackCoroutine { get; set; }

    /// <summary>
    /// Gets in-game encoder used to encode voice data.
    /// </summary>
    public OpusEncoder OpusEncoder { get; internal set; } = new(OpusApplicationType.Voip);

    /// <summary>
    /// Gets or sets current state of playback state.
    /// </summary>
    public PlaybackState State { get; set; } = PlaybackState.Stopped;

    /// <summary>
    /// Gets a value indicating whether player is stopped (not playing) or not.
    /// </summary>
    public bool IsStopped => this.State == PlaybackState.Stopped
        || this.State == PlaybackState.Paused;

    /// <summary>
    /// Gets currently playing track (can be url and file path).
    /// </summary>
    public string CurrentlyPlaying { get; private set; } = string.Empty;

    /// <summary>
    /// Gets current enqueued tracks.
    /// </summary>
    public List<string> EnqueuedTracks { get; private set; } = new();

    /// <summary>
    /// Playback coroutine.
    /// </summary>
    /// <returns>Time until new cycle.</returns>
    public IEnumerable<float> Playback()
    {
        while (true)
        {
            if (this.IsStopped && !this.EnqueuedTracks.Any())
            {
                yield return Timing.WaitUntilTrue(() => this.EnqueuedTracks.Any());
            }

            if (string.IsNullOrEmpty(this.CurrentlyPlaying))
            {
                var randomValue = this.Shuffle ? new Random().Next(0, this.EnqueuedTracks.Count) : 0;

                this.CurrentlyPlaying = this.EnqueuedTracks[randomValue];

                this.EnqueuedTracks.RemoveAt(randomValue);
            }

            // конвертим в 48000 1 channel
            var reader = new AudioFileReader(this.CurrentlyPlaying);
            using (var writer = new LameMP3FileWriter(reader, WaveFormat.CreateALawFormat(VoiceChatSettings.SampleRate, VoiceChatSettings.Channels), LAMEPreset.V9))
            {
                writer.CopyTo(reader);
            }
        }
    }

    /// <summary>
    /// Resume current playing track.
    /// </summary>
    public void Resume() => this.State = PlaybackState.Playing;

    /// <summary>
    /// Pause current playing track.
    /// </summary>
    public void Pause() => this.State = PlaybackState.Paused;

    /// <summary>
    /// Stop current playing track.
    /// </summary>
    public void Stop() => this.State = PlaybackState.Stopped;

    #endregion
}
