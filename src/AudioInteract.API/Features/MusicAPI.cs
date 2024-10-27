// <copyright file="MusicAPI.cs" company="Klybok Team">
// Copyright (c) Klybok Team. All rights reserved.
// </copyright>

namespace AudioInteract.Features;

using System.Runtime.CompilerServices;
using AudioInteract.API.Patches;
using CentralAuth;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Components;
using HarmonyLib;
using MEC;
using Mirror;
using PlayerRoles;
using UnityEngine;

/// <summary>
/// Main API class.
/// </summary>
public static class MusicAPI
{
    /// <summary>
    /// Gets harmony ID.
    /// </summary>
    public const string HarmonyID = "Klybok.Team - AudioInteract.API";

    /// <summary>
    /// Harmony main class.
    /// </summary>
    private static Harmony? harmony;
    private static bool isEventsRegistered = false;
    private static bool isInit = false;

    static MusicAPI()
    {
        isInit = true;

        if (Round.IsStarted)
        {
            Log.Error("Round have been already started and patches are subscribed later.");
            Log.Error("THIS MAY CAUSE CRITICAL BUGS.");
            Log.Error("To prevent this, please, apply patches on plugin start event by MusicAPI.EnsureInit().");
            Log.Error("Set it in every plugin, who uses music API.");
            Log.Error("Init will apply only once.");

            IDFix.AutoIncrement = RecyclablePlayerId._autoIncrement++;
        }

        harmony = new(HarmonyID);
        harmony.PatchAll();

        Exiled.Events.Handlers.Server.RestartingRound += IDFix.OnRestartRound_RefreshID;
    }

    /// <summary>
    /// Gets a value indicating whether events registered or not.
    /// </summary>
    public static bool IsEventsRegistered { get => isEventsRegistered; internal set => isEventsRegistered = value; }

    /// <summary>
    /// Gets settings of NPC.
    /// </summary>
    public static List<MusicInstance> MusicInstances { get; private set; } = new();

    /// <summary>
    /// Ensures that API is properly initialized.
    /// </summary>
    public static void EnsureInit()
    {
        if (!isInit)
        {
            RuntimeHelpers.RunClassConstructor(typeof(MusicAPI).TypeHandle);
        }
    }

    /// <summary>
    /// Creates new NPC and create settings for him.
    /// </summary>
    /// <param name="name">Name setted to NPC.</param>
    /// <param name="role">Role setted to NPC.</param>
    /// <param name="userID">UserID setted to NPC.</param>
    /// <returns>Returns Music Instance.</returns>
    public static MusicInstance CreateNPC(string name, RoleTypeId role = RoleTypeId.None, string userID = PlayerAuthenticationManager.DedicatedId)
    {
        Npc npc = CreateDefaultNPC(name, role, userID);

        return AddSetingsToNPC(npc);
    }

    /// <summary>
    /// Creates new NPC.
    /// </summary>
    /// <param name="name">Name setted to NPC.</param>
    /// <param name="role">Role setted to NPC.</param>
    /// <param name="userID">UserID setted to NPC. Default value hides NPC from list.</param>
    /// <returns>Returns Npc.</returns>
    public static Npc CreateDefaultNPC(string name, RoleTypeId role = RoleTypeId.None, string userID = PlayerAuthenticationManager.DedicatedId)
    {
        GameObject gameObject = Object.Instantiate(NetworkManager.singleton.playerPrefab);

        Npc npc = new Npc(gameObject)
        {
            IsNPC = true,
        };

        try
        {
            npc.ReferenceHub.roleManager.InitializeNewRole(RoleTypeId.None, RoleChangeReason.None);
        }
        catch (Exception arg)
        {
            Log.Debug($"Ignore: {arg}");
        }

        FakeConnection fakeConnection = new FakeConnection(int.MaxValue);

        NetworkServer.AddPlayerForConnection(fakeConnection, gameObject);

        try
        {
            npc.ReferenceHub.authManager.UserId = string.IsNullOrEmpty(userID) ? "Dummy@localhost" : userID;
        }
        catch (Exception arg2)
        {
            Log.Debug($"Ignore: {arg2}");
        }

        npc.ReferenceHub.nicknameSync.Network_myNickSync = name;
        Player.Dictionary.Add(gameObject, npc);

        Timing.CallDelayed(0.3f, () =>
        {
            npc.Role.Set(role);
        });

        npc.RemoteAdminPermissions = PlayerPermissions.AFKImmunity;

        try
        {
            if (userID == PlayerAuthenticationManager.DedicatedId)
            {
                npc.ReferenceHub.authManager.SyncedUserId = userID;
                try
                {
                    npc.ReferenceHub.authManager.InstanceMode = ClientInstanceMode.DedicatedServer;
                }
                catch (Exception e)
                {
                    Log.Debug($"Ignore: {e}");
                }
            }
            else
            {
                npc.ReferenceHub.authManager.UserId = userID == string.Empty ? $"Dummy@localhost" : userID;
            }
        }
        catch (Exception e)
        {
            Log.Debug($"Ignore: {e}");
        }

        return npc;
    }

    /// <summary>
    /// Create settings for NPC.
    /// </summary>
    /// <param name="npc">NPC to set settings.</param>
    /// <returns>Indicates settings is successfuly created or not.</returns>
    public static MusicInstance AddSetingsToNPC(Npc npc)
    {
        MusicInstances.Add(new(npc));

        return MusicInstances.FirstOrDefault(x => x.Npc == npc);
    }

    /// <summary>
    /// Destroy NPC.
    /// </summary>
    /// <param name="npc">NPC to destroy.</param>
    /// <returns>Indicates NPC is successfuly destroyed or not.</returns>
    public static bool DestroyNPC(Npc npc)
    {
        if (npc == null)
        {
            return false;
        }

        MusicInstance musicInstance = MusicInstances.FirstOrDefault(x => x.Npc == npc);
        if (musicInstance != null)
        {
            musicInstance.AudioPlayerBase?.OnDestroy();

            MusicInstances.Remove(musicInstance);
        }

        try
        {
            // lol
            npc.Disconnect();

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            return false;
        }
    }

    /// <summary>
    /// Destroy NPC.
    /// </summary>
    /// <param name="musicInstance">Music Instance.</param>
    /// <returns>Indicates NPC is successfuly destroyed or not.</returns>
    public static bool DestroyNPC(MusicInstance musicInstance)
    {
        if (musicInstance == null || musicInstance.Npc == null)
        {
            return false;
        }

        musicInstance.AudioPlayerBase?.OnDestroy();

        MusicInstances.Remove(musicInstance);

        try
        {
            musicInstance?.Npc.Destroy();

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            return false;
        }
    }
}
