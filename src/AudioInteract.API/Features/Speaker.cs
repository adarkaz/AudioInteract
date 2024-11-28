// <copyright file="Speaker.cs" company="Klybok Team">
// Copyright (c) Klybok Team. All rights reserved.
// </copyright>

namespace AudioInteract.API.Features;

using AdminToys;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using MEC;
using NAudio.Lame;
using NAudio.Wave;
using System.Reflection;
using UnityEngine.Windows;
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

        this.Base.Playback = new();
        this.Base.Playback.Buffer = new();

        this.PlaybackCoroutine = Timing.RunCoroutine(this.Playback());

        Instances.Add(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Speaker"/> class.
    /// </summary>
    /// <param name="speakerToy"><see cref="SpeakerToy"/>.</param>
    public Speaker(SpeakerToy speakerToy)
    {
        this.Base = new();

        this.PlaybackCoroutine = Timing.RunCoroutine(this.Playback());

        Instances.Add(this);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Speaker"/> class.
    /// </summary>
    ~Speaker()
    {
        Timing.KillCoroutines(this.PlaybackCoroutine);
    }

    /// <summary>
    /// Gets all instances <see cref="Speaker"/>.
    /// </summary>
    public static List<Speaker> Instances { get; internal set; } = new();

    /// <summary>
    /// Gets <see cref="SpeakerToy"/>.
    /// </summary>
    public SpeakerToy Base { get; private set; }

    /// <summary>
    /// Gets base playback buffer.
    /// </summary>
    public PlaybackBuffer Buffer => this.Base.Playback.Buffer;

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
    public CoroutineHandle PlaybackCoroutine { get; set; }

    /// <summary>
    /// Gets a value indicating whether player is stopped (not playing) or not.
    /// </summary>
    public bool IsStopped { get; internal set; } = false;

    /// <summary>
    /// Gets currently playing track (can be url and file path).
    /// </summary>
    public string CurrentlyPlaying { get; private set; } = string.Empty;

    /// <summary>
    /// Gets current enqueued tracks.
    /// </summary>
    public List<string> EnqueuedTracks { get; private set; } = new();

    /// <summary>
    /// Gets current play status.
    /// </summary>
    public PlayStatus Status { get; internal set; } = PlayStatus.Stopped;

    /// <summary>
    /// Playback coroutine.
    /// </summary>
    /// <returns>Time until new cycle.</returns>
    public IEnumerator<float> Playback()
    {
        while (true)
        {
            if (this.Status == PlayStatus.Playing)
            {
                yield break;
            }

            if (this.Base is null || this is null)
            {
                yield break;
            }

            if (!this.EnqueuedTracks.Any() || EnqueuedTracks.Count == 0)
            {
                yield return Timing.WaitUntilTrue(() => this.EnqueuedTracks.Any() && this.EnqueuedTracks.Count != 0);
            }

            if (string.IsNullOrEmpty(this.CurrentlyPlaying))
            {
                this.CurrentlyPlaying = this.EnqueuedTracks.FirstOrDefault();

                // Log.Info("posy3");
                // var randomValue = this.Shuffle ? new Random().Next(0, this.EnqueuedTracks.Count) : 0;
                //
                // this.CurrentlyPlaying = this.EnqueuedTracks[randomValue];

                // this.EnqueuedTracks.RemoveAt(randomValue);
            }

            /*
            // конвертим в 48000 1 channel
            var reader = new AudioFileReader(this.CurrentlyPlaying);
            using (var writer = new LameMP3FileWriter(reader, WaveFormat.CreateALawFormat(VoiceChatSettings.SampleRate, VoiceChatSettings.Channels), LAMEPreset.V9))
            {
                writer.CopyTo(reader);
            }*/
            try
            {
                Log.Info("posy6");

                var reader = new AudioFileReader(this.CurrentlyPlaying);

                using (var writer = new LameMP3FileWriter(reader, WaveFormat.CreateALawFormat(VoiceChatSettings.SampleRate, VoiceChatSettings.Channels), LAMEPreset.V9))
                {
                    writer.CopyTo(reader);
                }

                float[] buffer = new float[reader.Length / 2];

                reader.Read(buffer, 0, buffer.Length);

                this.Buffer.Write(buffer, buffer.Length);

                yield break;
                this.Status = PlayStatus.Playing;
            }
            catch(System.Exception ex)
            {
                Log.Error(ex);
            }
        }
    }

    /// <summary>
    /// Play track.
    /// </summary>
    /// <param name="path">Track path.</param>
    public void Play(string path)
    {
        if (!File.Exists(path))
        {
            Log.Warn($"[{Assembly.GetCallingAssembly().FullName}] Trying to play unexists file. Skipping.");

            return;
        }

        this.EnqueuedTracks.Add(path);
    }

    #endregion
}
