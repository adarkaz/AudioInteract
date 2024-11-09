// <copyright file="AudioPlayerParent.cs" company="Klybok Team">
// Copyright (c) Klybok Team. All rights reserved.
// </copyright>

namespace AudioInteract.Plugin.Commands;

using CommandSystem;

/// <summary/>
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class AudioPlayerParent : ParentCommand
{
    /// <inheritdoc/>
    public AudioPlayerParent()
    {
        this.LoadGeneratedCommands();
    }

    /// <summary>
    /// Gets or sets audio file.
    /// </summary>
    public static Dictionary<int> BotID { get; set; } = new();

    /// <summary>
    /// Gets or sets increment bot ID.
    /// </summary>
    public static int Increment { get; set; } = 0;

    /// <inheritdoc/>
    public override string Command { get; } = "audioplayer";

    /// <summary/>
    public override string Description { get; } = "Command to manage AudioInteract. Enter for more information.";

    /// <inheritdoc/>
    public override string[] Aliases { get; } = ["a-p", "au"];

    /// <inheritdoc/>
    public override void LoadGeneratedCommands()
    {
        this.RegisterCommand(new Add());
        this.RegisterCommand(new InstanceMode());
        this.RegisterCommand(new List());
        this.RegisterCommand(new Remove());
        this.RegisterCommand(new VoiceChannel());
        this.RegisterCommand(new Volume());
        this.RegisterCommand(new Play());
        this.RegisterCommand(new Stop());
    }

    /// <inheritdoc/>
    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        string responseTXT = "\n<color=white>Please, enter subcommand.</color>\n";

        foreach (KeyValuePair<string, ICommand> command in this.Commands)
        {
            responseTXT += $"\n<color=yellow>- {command.Value.Command} ({string.Join(", ", command.Value.Aliases)})</color>\n";
            responseTXT += $"<color=white>{command.Value.Description}</color>\n";
        }

        response = responseTXT;
        return true;
    }
}