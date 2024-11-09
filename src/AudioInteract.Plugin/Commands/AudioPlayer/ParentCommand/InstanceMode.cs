// <copyright file="InstanceMode.cs" company="Klybok Team">
// Copyright (c) Klybok Team. All rights reserved.
// </copyright>

namespace AudioInteract.Plugin.Commands;

using CentralAuth;
using CommandSystem;
using Exiled.API.Features;

/// <summary>
/// Gets or sets InstanceMode for bot.
/// </summary>
public class InstanceMode : ICommand
{
    /// <inheritdoc/>
    public string Command { get; } = "InstanceMode";

    /// <inheritdoc/>
    public string Description { get; } = "Gets or sets bot instance mode.";

    /// <inheritdoc/>
    public string[] Aliases { get; } = ["im", "i-m"];

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

        try
        {
            if (info.Npc.ReferenceHub.authManager.InstanceMode == ClientInstanceMode.DedicatedServer)
            {
                response = "Changed InstanceMode to Host.";
                info.Npc.ReferenceHub.authManager.InstanceMode = ClientInstanceMode.Host;
            }
            else
            {
                response = "Changed InstanceMode to Dedicated Server.";
                info.Npc.ReferenceHub.authManager.InstanceMode = ClientInstanceMode.DedicatedServer;
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            response = "Error when changing instance mode.";
            return false;
        }
    }
}