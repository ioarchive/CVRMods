using System.Reflection;
using ABI_RC.Helpers;
using MelonLoader;
using PresenceChanger;
using Main = PresenceChanger.Main;

[assembly: MelonInfo(typeof(Main), Bruh.Name, Bruh.Version, Bruh.Author)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]

namespace PresenceChanger;

public class Bruh
{
    public const string Name = "PresenceChanger";
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
        ListenToChanges();
        HarmonyInstance.Patch(
            typeof(PresenceManager).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance), null,
            typeof(Main).GetMethod(nameof(OnAwake)).ToNewHarmonyMethod());
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

    public static void OnAwake()
    {
        if (_appId.Value != _appId.DefaultValue)
            PresenceManager.instance.applicationId = _appId.Value;
    }

    public static void OnEnabled()
    {
        if (!_disablePresence.Value) return;
        PresenceManager.instance.enabled = false;
    }


    private static MelonPreferences_Category _category;
    private static MelonPreferences_Entry<bool> _disablePresence;

    private static MelonPreferences_Entry<string> _appId;

    // mm tasty
    private static void InitPrefs()
    {
        DebugMsg("Prefs init");
        _category = MelonPreferences.CreateCategory(Bruh.Name, "PresenceChanger");

        // 558834888594161666 is ChilloutVR
        _appId = _category.CreateEntry("DiscordAppId", "558834888594161666", "Discord App Id");
        _disablePresence = _category.CreateEntry("DisablePresence", false, "Disable Rich Presence");
    }

    private static void ListenToChanges()
    {
        // pls tell a better/cleaner way to do this - since this is kinda spaghetti
        logger.Msg("Listening to changes");
        _disablePresence.OnValueChanged += (o, n) =>
        {
            DebugMsg($"DisablePresence: {o} {n}");
            if (n == o) return;
            PresenceManager.instance.enabled = !n;
        };

        _appId.OnValueChanged += (o, n) =>
        {
            DebugMsg($"AppId: old: {o} new: {n}");
            if (n == o) return;
            PresenceManager.instance.applicationId = n;
            PresenceManager.instance.enabled = false;
            if (_disablePresence.Value) return;
            PresenceManager.instance.enabled = true;
            // restart presence to change app id
        };
    }

    private static void DebugMsg(string msg)
    {
        if (!MelonDebug.IsEnabled()) return;
        logger.Msg(msg);
    }
}