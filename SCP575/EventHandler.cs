using System;
using System.Collections.Generic;
using System.Linq;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using Respawning;

namespace SCP_575;

public static class EventHandler
{
    private static CoroutineHandle _blackoutHandler;
    private static Config Config => EntryPoint.Instance.Config;
    private static readonly Random Random = new();

    public static void RegisterEvents()
    {
        ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
        ServerEvents.RoundStarted += OnRoundStart;
    }

    public static void UnregisterEvents()
    {
        ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;
        ServerEvents.RoundStarted -= OnRoundStart;
    }

    private static void OnWaitingForPlayers()
    {
        // Remove the previous coroutine
        if (_blackoutHandler.IsRunning)
        {
            Timing.KillCoroutines(_blackoutHandler);
        }
    }

    private static void OnRoundStart()
    {
        Timing.CallDelayed(1f, () =>
        {
            if (Random.Next(100) >= Config.SpawnChance ||
                (Config.DisableForScp173 && Player.List.Any(x => x.Role == RoleTypeId.Scp173)))
            {
                return;
            }

            Logger.Info("SCP-575 will spawn in this round");
            _blackoutHandler = Timing.RunCoroutine(Blackout());
        });
    }

    private static IEnumerator<float> Blackout()
    {
        // if (Config.BlackOut.RandomInitialDelay)
        // {
        //     var delay = Random.Next((int)Config.BlackOut.InitialMinDelay, (int)Config.BlackOut.InitialMaxDelay);
        //
        //     Logger.Debug($"{nameof(Blackout)}: Random delay activated, waiting for {delay} seconds", Config.DebugMode);
        //     yield return Timing.WaitForSeconds(delay);
        // }
        // else
        // {
        //     Logger.Debug($"{nameof(Blackout)}: Waiting for {Config.BlackOut.InitialDelay} seconds", Config.DebugMode);
        //     yield return Timing.WaitForSeconds(Config.BlackOut.InitialDelay);
        // }

        while (Round.IsRoundStarted)
        {
            // Obtains the blackout duration by calculating between the minimum and maximum duration.
            var blackoutDuration =
                (float)Random.NextDouble() * (Config.BlackOut.MaxDuration - Config.BlackOut.MinDuration) +
                Config.BlackOut.MinDuration;

            if (!string.IsNullOrEmpty(Config.BlackOut.Cassie?.Message) && Config.BlackOut.Cassie != null)
            {
                // Send Cassie's message to everyone

                RespawnEffectsController.PlayCassieAnnouncement(Config.BlackOut.Cassie?.Message,
                    Config.BlackOut.Cassie.IsHeld, Config.BlackOut.Cassie.IsNoisy, Config.BlackOut.Cassie.IsSubtitle);
            }

            // Wait for Cassie to finish speaking
            yield return Timing.WaitForSeconds(Config.BlackOut.DelayAfterCassie);

            var antiScp173 = false;
            if (Config.DisableForScp173)
            {
                antiScp173 = Player.List.Any(x => x.Role == RoleTypeId.Scp173);
            }

            // Turn off the lights in the area
            BlackoutExtensions.StartBlackout(blackoutDuration, antiScp173);

            try
            {
                // Spawn SCP-575

                var victim = GetVictim();

                if (victim != null)
                {
                    BlackoutExtensions.SpawnScp575(victim, blackoutDuration);
                }

                if (victim is null)
                {
                    Logger.Debug($"{nameof(BlackoutExtensions.SpawnScp575)}: victim player is null.", Config.DebugMode);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error on {nameof(BlackoutExtensions.SpawnScp575)}: {e}");
            }

            // Decide the delay by calculating between the minimum and the maximum value.
            yield return Timing.WaitForSeconds(Random.Next(Config.BlackOut.MinDelay, Config.BlackOut.MaxDelay) +
                                               blackoutDuration);
        }
    }

    /// <summary>
    /// Obtains a potential victim player based on specified blackout zones and certain conditions.
    /// </summary>
    /// <returns>A potential victim player or null if no valid player is found.</returns>
    private static Player GetVictim()
    {
        try
        {
            // Get all players and filter based on specific conditions.
            var players = Player.List
                .Where(player =>
                    player.IsAlive &&
                    !player.IsSCP &&
                    !player.IsTutorial &&
                    !player.InInvalidRoom());

            // Get the active blackout zones from the configuration.
            var activeZones = Config.BlackOut.ActiveZones;

            // Filter players based on active blackout zones.
            if (activeZones.Count > 0)
            {
                players = players.Where(player => activeZones.Contains(player.Zone));
            }

            if (activeZones.Count == 0)
            {
                Logger.Error($"{nameof(GetVictim)}: Config.BlackOut.ActiveZones is 0");
                return null;
            }

            var filteredPlayers = players.ToList();

            // Return a potential victim player if any, otherwise, return null.
            return filteredPlayers.Any()
                ? filteredPlayers.ElementAtOrDefault(UnityEngine.Random.Range(0, filteredPlayers.Count))
                : null;
        }
        catch (Exception e)
        {
            // Log any errors that occur during the process.
            Logger.Error($"Error on {nameof(GetVictim)}: {e} -- {e.Message}");
            return null;
        }
    }
}