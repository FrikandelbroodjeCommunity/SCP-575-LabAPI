using System;
using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using FrikanUtils.Audio;
using FrikanUtils.Npc;
using FrikanUtils.Npc.Enums;
using FrikanUtils.Npc.Following;
using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;
using Random = System.Random;

namespace SCP_575;

/// <summary>
/// Extension methods for handling blackout in the game.
/// </summary>
public static class BlackoutExtensions
{
    /// <summary>
    /// List of room light controllers affected during the last blackout.
    /// </summary>
    private static readonly List<FacilityZone> ZonesAffected = new();

    private static Config Config => EntryPoint.Instance.Config;
    private static LightsComponent Lights => EntryPoint.Component;
    private static readonly Random Random = new();

    /// <summary>
    /// Initiates a blackout, causing flickering lights in specified zones.
    /// </summary>
    /// <param name="duration">Duration of the blackout in seconds.</param>
    /// <param name="scp173">Flag indicating if SCP-173 behavior should be considered.</param>
    public static void StartBlackout(float duration, bool scp173 = false)
    {
        Logger.Info("Starting a blackout");

        // Clear the list of affected lights from the previous blackout.
        ZonesAffected.Clear();

        foreach (FacilityZone zone in Enum.GetValues(typeof(FacilityZone)))
        {
            if (!Config.BlackOut.ActiveZones.Contains(zone) ||
                scp173 && zone == FacilityZone.HeavyContainment) continue;

            Map.TurnOffLights(duration, zone);
            ZonesAffected.Add(zone);
        }

        foreach (var room in Config.BlackOut.BlackListRooms.SelectMany(Room.Get))
        {
            room.LightController.FlickerLights(0);
        }
    }

    /// <summary>
    /// Ends the blackout, restoring lights to their normal state.
    /// </summary>
    private static void EndBlackout()
    {
        foreach (var zone in ZonesAffected)
        {
            Map.TurnOnLights(zone);
        }

        ZonesAffected.Clear();
        Lights.Npc = null; // Make very sure there is no reference anymore
    }

    /// <summary>
    /// Checks if the specified room is illuminated.
    /// </summary>
    /// <param name="roomId">The identifier of the room to check.</param>
    /// <returns>
    /// True if the room is illuminated; otherwise, false.
    /// </returns>
    public static bool IsRoomIlluminated(RoomIdentifier roomId)
    {
        // Attempt to find the RoomLightController component in the child objects of the specified room.
        var lightController = roomId.GetComponentInChildren<RoomLightController>();

        // Check if a RoomLightController is found and its network lights are enabled.
        // Assumes that the state of network lights determines the illumination of the room.
        return lightController != null && lightController.NetworkLightsEnabled;
    }

    /// <summary>
    /// Checks if the victim is in a blacklisted room based on the current configuration.
    /// </summary>
    /// <param name="player">The victim to check.</param>
    /// <returns>
    /// True if the victim is in a blacklisted room; otherwise, false.
    /// </returns>
    public static bool InInvalidRoom(this Player player)
    {
        // If the victim or room is null, or the client instance is not ready, consider it not in an invalid room.
        if (player == null || player.Room == null || !player.IsPlayer)
        {
            return false;
        }

        // Check if the current room is blacklisted.
        return Config.BlackOut.BlackListRooms.Contains(player.Room.Name);
    }

    /// <summary>
    /// Handler the spawn of a SCP-575 instance.
    /// </summary>
    /// <param name="victim"></param>
    /// <param name="duration"></param>
    public static void SpawnScp575(Player victim, float duration)
    {
        try
        {
            if (victim is null)
            {
                Logger.Debug($"{nameof(SpawnScp575)}: victim victim not found.", EntryPoint.Instance.Config.DebugMode);
                return;
            }

            Logger.Info($"Starting to spawn SCP-757 on {victim.LogName}");

            var dummy = NpcSystem.CreateHiddenDummy(Config.Scp575.Nickname);
            dummy.SetRole(Config.Scp575.RoleType);
            dummy.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;
            dummy.ReferenceHub.nicknameSync.ViewRange = EntryPoint.Instance.Config.Scp575.ViewRange;

            // Add the chase component on a delay if needed
            if (Config.Scp575.DelayOnChase)
            {
                Timing.CallDelayed(Config.Scp575.DelayChase,
                    () => { AddFollowComponent(dummy, victim, duration - Config.Scp575.DelayChase); });
            }
            else
            {
                AddFollowComponent(dummy, victim, duration);
            }

            // Small delay between setting the role of the dummy and moving it to the desired room
            Timing.CallDelayed(0.3f, () =>
            {
                Logger.Debug("Moving dummy to the victim's room", EntryPoint.Instance.Config.DebugMode);

                var room = victim.Room;

                if (room.Name == RoomName.Lcz173)
                {
                    dummy.Position = room.Position + new Vector3(0f, 13.5f, 0f);
                }
                else if (room.Name == RoomName.HczTestroom)
                {
                    if (DoorVariant.DoorsByRoom.TryGetValue(room.Base, out var hashSet))
                    {
                        var door = hashSet.FirstOrDefault();
                        if (door != null) dummy.Position = door.transform.position + Vector3.up;
                    }
                }
                else
                {
                    if (room.Zone == FacilityZone.Surface)
                    {
                        dummy.Position = victim.Position + Vector3.back;
                    }
                    else
                    {
                        dummy.Position = room.Position + Vector3.up;
                    }
                }
            });

            // Try to start the audio
            if (!Config.Scp575.PlaySounds || Config.AudioFiles.IsEmpty())
            {
                return;
            }

            var random = Config.AudioFiles[Random.Next(Config.AudioFiles.Length)];
            var audioPlayer = new PlayerSpeakerAudioPlayer(dummy);
            audioPlayer.Queue([random], true);
            audioPlayer.OverrideVolume = Config.Scp575.SoundVolume;
            audioPlayer.Looping = Config.Scp575.AudioIsLooped;

            Logger.Debug($"Playing sound {random}", EntryPoint.Instance.Config.DebugMode);
        }
        catch (Exception e)
        {
            Logger.Error($"Error while trying to spawn SCP-575: {e}");
        }
    }

    private static void AddFollowComponent(Player player, Player victim, float duration)
    {
        Logger.Info("Setting up following component");
        var scp575 = new FollowingNpc(player, victim)
        {
            WalkSpeed = Config.Scp575.MovementSpeed,
            SprintSpeed = Config.Scp575.MovementSpeedFast,
            IdleDistance = Config.Scp575.KillDistance,
            SprintDistance = Config.Scp575.MediumDistance,
            MaxDistance = Config.Scp575.MaxDistance,
            OutOfRangeAction = OutOfRangeAction.Destroy,
            ReachTargetAction = ReachTargetAction.CustomAction,
            OnDestroy = _ =>
            {
                Lights.Npc = null;

                if (Config.BlackOut.EndBlackoutWhenDisappearing)
                {
                    EndBlackout();
                }
            },
            ReachedTarget = () => { victim.Kill(Config.Scp575.KillReason); }
        };

        if (Config.Scp575.UseLightPoints)
        {
            Lights.Npc = scp575;
            Lights.Score = 0;
        }

        var comp = scp575.Dummy.GameObject.AddComponent<CheckerComponent>();
        comp.Npc = scp575;

        if (Config.Scp575.RoleType != PlayerRoles.RoleTypeId.Scp106 &&
            Config.Scp575.GiveGhostlyEffect)
        {
            scp575.Dummy.EnableEffect<Ghostly>();
        }

        Timing.CallDelayed(duration, () => { scp575.Destroy(DestroyReason.Removal); });
    }
}