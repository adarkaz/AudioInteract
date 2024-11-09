// <copyright file="Stop.cs" company="Klybok Team">
// Copyright (c) Klybok Team. All rights reserved.
// </copyright>

namespace AudioInteract.Plugin.Commands;

using CommandSystem;

/// <summary>
/// Stop music.
/// </summary>
public class Stop : ICommand
{
    /// <inheritdoc/>
    public string Command { get; } = "stop";

    /// <inheritdoc/>
    public string Description { get; } = "Stop playing music.";

    /// <inheritdoc/>
    public string[] Aliases { get; } = ["s"];

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

        info.Stop();

        response = $"Successfully stoped playing music.";
        return true;
    }
}