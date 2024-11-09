// <copyright file="AudioFile.cs" company="Klybok Team">
// Copyright (c) Klybok Team. All rights reserved.
// </copyright>

namespace AudioInteract.Features;

using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using UnityEngine;
using VoiceChat;
using YamlDotNet.Serialization;

/// <summary>
/// Creates audio file to easy interact with API.
/// </summary>
public class AudioFile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioFile"/> class.
    /// </summary>
    /// <param name="filePath">Path to played file.</param>
    /// <param name="isEnabled">Indicates enabled <see cref="AudioFile"/> or not.</param>
    /// <param name="isLooped">Is track looped or not.</param>
    /// <param name="volume">Volume of track.</param>
    /// <param name="voicechannel">Voice channel of track.</param>
    /// <param name="botname">Bot name.</param>
    public AudioFile(string filePath, bool isEnabled = true, bool isLooped = false, int volume = 75, VoiceChatChannel voicechannel = VoiceChatChannel.Intercom, string botname = "")
    {
        this.FilePath = filePath;
        this.IsLooped = isLooped;
        this.Volume = volume;
        this.VoiceChannel = voicechannel;
        this.BotName = botname;
        this.IsEnabled = isEnabled;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioFile"/> class.
    /// </summary>
    public AudioFile()
    {
    }

    /// <summary>
    /// Gets default path to audio file directory, leads to EXILED root directory (EXILED/Audio).
    /// </summary>
    [YamlIgnore]
    public static string RawFilePath => Extensions.FolderPath;

    /// <summary>
    /// Gets or sets a value indicating whether this audio file enabled or not.
    /// </summary>
    [Description("Indicates enabled AudioFile or not.")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating bot name.
    /// </summary>
    [Description("Sets a value indicating bot name.")]
    public string BotName { get; set; } = "Bot";

    /// <summary>
    /// Gets or sets path to audio file. Default leads to EXILED root directory (EXILED/Audio/track.ogg).
    /// </summary>
    [Description("Sets file path to music.")]
    public string FilePath { get; set; } = Path.Combine(Paths.Exiled, "Audio", "track.ogg");

    /// <summary>
    /// Gets or sets a value indicating whether loop track or not. Default is false.
    /// </summary>
    [Description("Indicates enabled AudioFile is looped or not.")]
    public bool IsLooped { get; set; } = false;

    /// <summary>
    /// Gets or sets value that indicates volume of track. Default is 75.
    /// </summary>
    [Description("Set volume of track.")]
    public int Volume { get; set; } = 75;

    /// <summary>
    /// Gets or sets a value indicating whether bot noclip enabled or not.
    /// </summary>
    [Description("Indicates noclip of bot is enabled or not.")]
    public bool IsNoclipEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets scale of bot.
    /// </summary>
    [Description("Sets scale of bot.")]
    public Vector3 Scale { get; set; } = new(0, 0, 0);

    /// <summary>
    /// Gets or sets <see cref="VoiceChatChannel"/> of bot. Default is Intercom.
    /// </summary>
    [Description("Get the voice channel of bot. See https://github.com/klybok-team/AudioInteract/blob/main/src/AudioInteract.Plugin/README.md for more info.")]
    public VoiceChatChannel VoiceChannel { get; set; } = VoiceChatChannel.Intercom;

    /// <summary>
    /// Gets or sets current bot role. Leave overwatch to not spawn bot.
    /// </summary>
    [Description("Gets or sets current bot role. Leave any non-playable role (spectrator, etc) to not spawn bot.")]
    public RoleTypeId RoleType { get; set; } = RoleTypeId.Overwatch;

    /// <summary>
    /// Gets or sets current bot position. Leave to not spawn bot.
    /// </summary>
    [Description("Gets or sets current bot position. Leave x, y, z zero or null to not spawn.")]
    public Vector3 Postion { get; set; } = new(0, 0, 0);

    /// <summary>
    /// Gets or sets current bot room type to set local position. Leave to not spawn bot.
    /// </summary>
    [Description("Gets or sets current bot room type. Leave null to not spawn bot in room.")]
    public RoomType RoomType { get; set; } = RoomType.Unknown;

    /// <summary>
    /// Gets or sets current bot local position for room. Leave to not spawn bot.
    /// </summary>
    [Description("Gets or sets current bot LOCAL room position. Leave x, y, z zero or null to not spawn, local-room-position command to get.")]
    public Vector3 LocalRoomPostion { get; set; } = new(0, 0, 0);
}
