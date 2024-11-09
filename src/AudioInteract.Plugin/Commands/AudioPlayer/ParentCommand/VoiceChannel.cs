// <copyright file="VoiceChannel.cs" company="Klybok Team">
// Copyright (c) Klybok Team. All rights reserved.
// </copyright>

namespace AudioInteract.Plugin.Commands;

using CommandSystem;
using VoiceChat;

/// <summary>
/// Gets or sets voice channel for bot.
/// </summary>
public class VoiceChannel : ICommand
{
    /// <inheritdoc/>
    public string Command { get; } = "voicechannel";

    /// <inheritdoc/>
    public string Description { get; } = "Gets or sets bot voice channel.";

    /// <inheritdoc/>
    public string[] Aliases { get; } = ["vc"];

    /// <inheritdoc/>
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count < 1)
        {
            response = "Enter ID of bot, get IDs of bots: a-p list";
            return false;
        }

        if (!int.TryParse(arguments.At(0), out int search_value))
        {
            response = "Enter ID of bot, get IDs of bots: a-p list";
            return false;
        }

        if (!AudioPlayerParent.BotID.TryGetValue(search_value, out Features.MusicInstance_Old? info))
        {
            response = "Bot not found.";
            return false;
        }

        if (arguments.Count < 2 || !Enum.TryParse<VoiceChatChannel>(arguments.At(1), out VoiceChatChannel voiceChannel))
        {
            response = "Enter voice channel of bot. There currently:"
                + $"\n{VoiceChatChannel.Proximity}"
                + $"\n{VoiceChatChannel.Radio} (sound modification, hear all with Radio)"
                + $"\n{VoiceChatChannel.ScpChat} (only SCP can hear it)"
                + $"\n{VoiceChatChannel.Spectator} (hear only spectrators?)"
                + $"\n{VoiceChatChannel.RoundSummary} (hear all)"
                + $"\n{VoiceChatChannel.Intercom} (sound modification)"
                + $"\n{VoiceChatChannel.Mimicry}"
                + $"\n{VoiceChatChannel.Scp1576} (sound modification)"
                + $"\n{VoiceChatChannel.PreGameLobby} (hear all)";
            return false;
        }

        info.VoiceChatChannel = voiceChannel;

        response = $"Setted bot channel to {voiceChannel}.";
        return true;
    }
}