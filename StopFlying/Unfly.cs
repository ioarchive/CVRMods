using System.Collections;
using ABI_RC.Core.IO;
using ABI_RC.Systems.MovementSystem;
using MelonLoader;
using StopFlying;

[assembly: MelonInfo(typeof(Unfly), Build.Name, Build.Version, Build.Author)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]

namespace StopFlying;

public static class Build
{
    public const string Name = "StopFlying";
    public const string Author = "Animal";
    public const string Version = "1.0.0";
}

public class Unfly : MelonMod
{
    public override void OnApplicationStart()
    {
        HarmonyInstance.Patch(
            typeof(CVRObjectLoader).GetMethod(nameof(CVRObjectLoader.LoadIntoWorld)),
            null, typeof(Unfly).GetMethod(nameof(DisableFlying)).ToNewHarmonyMethod());
    }

    public static void DisableFlying(DownloadJob.ObjectType __0, string __1, byte[] __2)
    {
        MelonCoroutines.Start(WaitForWorld());
    }

    private static IEnumerator WaitForWorld()
    {
        // was too early before
        while (CVRObjectLoader.Instance.IsLoadingWorldToJoin) yield return null;
        MovementSystem.Instance.ChangeFlight(false);
    }
}