// <copyright file="Plugin.cs" company="Klybok Team">
// Copyright (c) Klybok Team. All rights reserved.
// </copyright>

namespace AudioInteract.Plugin.Configs;

using System.ComponentModel;
using AudioInteract.Features;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using PlayerRoles;
using Respawning;

/// <summary/>
public sealed class Plugin : IConfig
{
    /// <summary/>
    [Description("Indicates plugin is enabled or not")]
    public bool IsEnabled { get; set; } = true;

    /// <summary/>
    [Description("Indicates debug mode is enabled or not")]
    public bool Debug { get; set; } = true;

    /// <summary/>
    [Description("Indicates will plugin register and try to play music or not.")]
    public bool IsEventsEnabled { get; set; } = true;

    /// <summary/>
    [Description("Plays music in lobby. Remove value IsEnabled or set value to true to enable it.")]
    public List<AudioFile> LobbyMusic { get; set; } = new()
    {
        {
            new()
            {
                IsEnabled = false,
                BotName = "Lobby Music",
                FilePath = Path.Combine(Paths.Config, "lobby_music.ogg"),
                IsLooped = true,
                VoiceChannel = VoiceChat.VoiceChatChannel.Intercom,
                Volume = 75,
                RoleType = RoleTypeId.Overwatch,
            }
        },
    };

    /// <summary/>
    [Description("Plays music when warhead stared. Remove value IsEnabled or set value to true to enable it.")]
    public AudioFile WarheadMusic { get; set; } = new()
    {
        IsEnabled = false,
        BotName = "Warhead Music",
        FilePath = Path.Combine(Paths.Config, "warhead_music.ogg"),
        IsLooped = true,
        VoiceChannel = VoiceChat.VoiceChatChannel.Intercom,
        Volume = 75,
        RoleType = RoleTypeId.Overwatch,
    };

    /// <summary/>
    [Description("Plays music when team respawns. Remove value IsEnabled or set value to true to enable it.")]
    public Dictionary<SpawnableTeamType, AudioFile> TeamMusic { get; set; } = new()
    {
        {
            SpawnableTeamType.NineTailedFox,
            new()
            {
                IsEnabled = false,
                BotName = "MTF",
                FilePath = Path.Combine(Paths.Config, "mtf.ogg"),
                IsLooped = false,
                VoiceChannel = VoiceChat.VoiceChatChannel.Intercom,
                Volume = 75,
                RoleType = RoleTypeId.Overwatch,
            }
        },
        {
            SpawnableTeamType.ChaosInsurgency,
            new()
            {
                IsEnabled = false,
                BotName = "CI",
                FilePath = Path.Combine(Paths.Config, "ci.ogg"),
                IsLooped = false,
                VoiceChannel = VoiceChat.VoiceChatChannel.Intercom,
                Volume = 75,
                RoleType = RoleTypeId.Overwatch,
            }
        },
    };
}