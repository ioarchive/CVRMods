using System;
using System.Collections;
using System.Reflection;
using ABI_RC.Core;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Networking.IO.Social;
using ABI_RC.Core.Player;
using ABI_RC.Core.UI;
using ABI_RC.Helpers;
using Harmony;
using MelonLoader;
using PresenceCustomizer;
using UnityEngine;
using Main = PresenceCustomizer.Main;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(Main), Bruh.Name, Bruh.Version, Bruh.Author)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]

namespace PresenceCustomizer;

public class Bruh
{
    public const string Name = "PresenceCustomizer";
    public const string Version = "1.0.0";
    public const string Author = "Animal";
}

public class Main : MelonMod
{
    private static MelonLogger.Instance logger;

    public override void OnApplicationStart()
    {
        logger = LoggerInstance;
        InitPrefs();
        HarmonyInstance.Patch(
            typeof(PresenceManager).GetMethod("OnEnable", BindingFlags.NonPublic | BindingFlags.Instance), null,
            typeof(Main).GetMethod(nameof(OnEnabled)).ToNewHarmonyMethod());
        /*MelonCoroutines.Start(InitUi());*/
    }

    // maybe later when ui is easier to use
    /*private static IEnumerator InitUi()
    {
        while (GameObject.Find("Cohtml/QuickMenu") == null) yield return null;
        ViewManager.Instance.gameMenuView.View.ExecuteScript("cvr.menu.prototype.testmod");
    }*/

    public static void OnEnabled()
    {
        logger.Msg("PresenceCustomizer enabled");
        if (!PresenceEnabled.Value) PresenceManager.instance.enabled = false;
    }


    private static MelonPreferences_Category category;

    public static MelonPreferences_Entry<string> Details,
        State,
        AppId,
        LargeImageKey,
        LargeImageText,
        SmallImageKey,
        SmallImageText;

    public static MelonPreferences_Entry<bool> PresenceEnabled;

    public override void OnPreferencesSaved()
    {
        PresenceManager.UpdatePresence(Details.Value, State.Value, largeKey: LargeImageKey.Value,
            largeText: LargeImageText.Value, smallKey: SmallImageKey.Value, smallText: SmallImageText.Value);
    }

    private static void InitPrefs()
    {
        logger.Msg("Prefs init");
        category = MelonPreferences.CreateCategory(Bruh.Name, "PresenceCustomizer");
        // 558834888594161666 is ChilloutVR
        AppId = category.CreateEntry("DiscordAppId", "558834888594161666", "Discord App Id");
        logger.Msg("bruh");
        PresenceEnabled = category.CreateEntry("PresenceEnabled", true, "Rich Presence Enabled");
        Details = category.CreateEntry("PresDetails", string.Empty, "Presence Details");
        State = category.CreateEntry("PresState", string.Empty, "Presence State");
        LargeImageKey = category.CreateEntry("LargeImageKey", string.Empty, "Large Image Key");
        SmallImageKey = category.CreateEntry("SmallImageKey", string.Empty, "Small Image Key");
        LargeImageText = category.CreateEntry("LargeImageText", string.Empty, "Large Image Text");
        SmallImageText = category.CreateEntry("SmallImageText", string.Empty, "Small Image Text");

        logger.Msg("pe");
        PresenceEnabled.OnValueChanged += (o, n) =>
        {
            logger.Msg($"PresenceEnabled: {o} {n}");
            if (n == o) return;
            PresenceManager.instance.enabled = n;
        };

        logger.Msg("App Id");

        AppId.OnValueChanged += (o, n) =>
        {
            logger.Msg($"AppId: {o} {n}");
            if (n == o) return;
            PresenceManager.instance.applicationId = n;
            // restart presence to change app id
            PresenceManager.instance.enabled = false;
            PresenceManager.instance.enabled = PresenceEnabled.Value;
        };
    }
}