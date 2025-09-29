using System;
using System.Linq;
using FrikanUtils.Npc.Enums;
using FrikanUtils.Npc.Following;
using LabApi.Features.Wrappers;
using PlayerRoles.PlayableScps;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SCP_575;

public class LightsComponent : MonoBehaviour
{
    [NonSerialized] public FollowingNpc Npc;
    [NonSerialized] public int Score;

    private const int MaxViewDistance = 25;
    private static Config Config => EntryPoint.Instance.Config;
    private float _timer = 0.5f;

    private void Update()
    {
        if (Npc == null)
        {
            return;
        }

        _timer -= Time.deltaTime;
        if (_timer > 0)
        {
            return;
        }
        

        _timer = 0.5f;
        var dummyPos = Npc.Dummy.Position;
        foreach (var player in Player.List.Where(HasLight))
        {
            if (!VisionInformation.GetVisionInformation(player.ReferenceHub, player.Camera, dummyPos, 1,
                    MaxViewDistance, false, false).IsLooking)
            {
                continue;
            }

            if (!Physics.Linecast(dummyPos, player.Position, VisionInformation.VisionLayerMask))
            {
                Logger.Info("Player is looking at scp-575");
                Score++;
            }
        }

        if (Score >= Config.Scp575.LightPoints)
        {
            Logger.Info("Threshold reached");
            Npc.Destroy(DestroyReason.Cleanup);
        }
    }

    private static bool HasLight(Player player)
    {
        return player.CurrentItem switch
        {
            FlashlightItem flashlight => flashlight.IsEmitting,
            FirearmItem firearm => firearm.FlashlightEnabled,
            _ => false
        };
    }
}