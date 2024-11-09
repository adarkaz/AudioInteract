using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using VoiceChat.Networking;
using YamlDotNet.Core.Tokens;

namespace VoiceChat.Playbacks;

public class SpeakerToyPlaybackBase : VoiceChatPlaybackBase
{
    public Vector3 LastPosition
    {
        get
        {
            return default(Vector3);
        }
        private set
        {
        }
    }

    public bool Culled
    {
        get
        {
            return default(bool);
        }
        private set
        {
        }
    }

    public override int MaxSamples
    {
        get
        {
            return 0;
        }
    }

    private static void Init()
    {
    }

    private static void OnLateUpdate()
    {
    }

    private static void ValidatePlayback(Vector3 cameraPosition, SpeakerToyPlaybackBase playback, ref int validSpeakers)
    {
    }

    public override float ReadSample()
    {
        throw new NotImplementedException();
    }

    public SpeakerToyPlaybackBase()
    {
    }

    public static readonly HashSet<SpeakerToyPlaybackBase> AllInstances;

    private const int MaxAudibleSpeakers = 4;

    private static readonly SpeakerToyPlaybackBase[] AudibleSpeakerInst;

    private static readonly float[] AudibleSpeakersDis;

    public int ControllerId;

    public PlaybackBuffer Buffer;
}
