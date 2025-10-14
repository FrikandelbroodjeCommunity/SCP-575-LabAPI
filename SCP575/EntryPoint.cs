using System;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SCP_575;

/// <summary>
/// Plugin main class
/// </summary>
public class EntryPoint : Plugin<Config>
{
    public override string Name => "SCP-575";
    public override string Description => "Add SCP-575 as an NPC that pursues players.";
    public override string Author => "SrLicht";
    public override Version Version => new(1, 0, 2);
    public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;

    public static EntryPoint Instance { get; private set; }

    public static LightsComponent Component { get; private set; }

    public override void Enable()
    {
        Instance = this;
        
        if (Config.Scp575.UseLightPoints)
        {
            Component = new GameObject().AddComponent<LightsComponent>();
            Object.DontDestroyOnLoad(Component);
        }

        EventHandler.RegisterEvents();
    }

    public override void Disable()
    {
        EventHandler.UnregisterEvents();

        if (Component != null)
        {
            Object.Destroy(Component.gameObject);
        }
    }
}