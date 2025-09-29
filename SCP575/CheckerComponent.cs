using FrikanUtils.Npc.Enums;
using FrikanUtils.Npc.Following;
using UnityEngine;

namespace SCP_575;

public class CheckerComponent : MonoBehaviour
{
    public const float CheckInterval = 1f;

    public FollowingNpc Npc;

    private float _timer = CheckInterval;

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer > 0) return;
        _timer = CheckInterval;

        if (BlackoutExtensions.IsRoomIlluminated(Npc.Dummy.Room.Base))
        {
            Npc.Destroy(DestroyReason.Removal);
        }
    }
}