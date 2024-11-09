// <copyright file="Play.cs" company="Klybok Team">
// Copyright (c) Klybok Team. All rights reserved.
// </copyright>

namespace AudioInteract.Plugin.Commands;

using AudioInteract.Features;
using CommandSystem;

/// <summary>
/// Play music.
/// </summary>
public class Play : ICommand
{
    /// <inheritdoc/>
    public string Command { get; } = "play";

    /// <inheritdoc/>
    public string Description { get; } = "Play music.";

    /// <inheritdoc/>
    public string[] Aliases { get; } = ["p"];

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

        if (!AudioPlayerParent.BotID.TryGetValue(search_value, out MusicInstance_Old? info))
        {
            response = "Bot not found.";
            return false;
        }

        if (arguments.Count < 2)
        {
            response = "Enter path to audio file.";
            return false;
        }

        // skipping first
        string path = string.Join(" ", arguments.Skip(1));

        if (!path.IsValidFileOrValidUrl())
        {
            response = $"File on path don't exists.";
            return true;
        }

        info.Play(path);

        response = $"Playing track at path - {path}";
        return true;
    }
}