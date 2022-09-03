using ABI_RC.Core;
using ABI_RC.Core.Player;
using RestartButton;
using MelonLoader;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using ButtonAPI = ChilloutButtonAPI.ChilloutButtonAPIMain;
using Main = RestartButton.Main;

[assembly: MelonInfo(typeof(Main), Guh.Name, Guh.Version, Guh.Author, Guh.DownloadLink)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]

namespace RestartButton;

public static class Guh
{
    public const string Name = "RestartButton";
    public const string Author = "Animal & Bluscream";
    public const string Version = "1.1.1";
    public const string DownloadLink = "https://github.com/Aniiiiiimal/CVRMods";
}

public class Main : MelonMod
{
    private const string BatTemplate = @"
taskkill /f /im {0}
timeout /t {1}
start """" {2}
";

    private MelonPreferences_Entry keybind;
    private MelonPreferences_Entry vrFailsafe;

    private MelonLogger.Instance logger;

    public override void OnApplicationStart()
    {
        logger = LoggerInstance;
        var cat = MelonPreferences.CreateCategory(Guh.Name);
        keybind = cat.CreateEntry("restart_bind", KeyCode.End, "Restart Key Bind",
            "Key to press to restart game");
        vrFailsafe = cat.CreateEntry("vr_failsafe", false, "VR Failsafe",
            "Failsafe option to detect VR even if the command line arguments don't provide it");
        ButtonAPI.OnInit += () =>
        {
            _ = ButtonAPI.MainPage.AddButton("Restart", "Restart ChilloutVR", RestartGame);
        };
    }

    private void RestartGame()
    {
        logger.Warning("Restarting!");
        MelonPreferences.Save();
        var filename = $"{Process.GetCurrentProcess().ProcessName}.exe";
        var args = $"{string.Join(" ", Environment.GetCommandLineArgs()).Replace($"{Environment.CurrentDirectory}\\ChilloutVR.exe", "")}";
        // MelonLogger.Warning(PlayerSetup.Instance._inVr ? "IN VR" : "NOT IN VR");
        // MelonLogger.Warning(PlayerSetup.Instance.vrHeadTracker.activeSelf ? "VR HEADSET ACTIVE" : "VR HEADSET INACTIVE");
        logger.Warning("Game start args: " + args);
        if ((bool) vrFailsafe.BoxedValue && PlayerSetup.Instance._inVr)
        {
            args += " -vr";
        }
        
        File.WriteAllText("restart.bat", string.Format(BatTemplate, filename, 3, $"\"{Environment.CurrentDirectory}\\ChilloutVR.exe\" {args}"));
        _ = Process.Start(new ProcessStartInfo
            {FileName = "restart.bat", CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden});
        RootLogic.Instance.QuitApplication();
        Environment.Exit(0);
    }

    public override void OnUpdate()
    {
        if (!Input.GetKeyDown((KeyCode) keybind.BoxedValue))
        {
            return;
        }

        try
        {
            RestartGame();
        }
        catch (Exception e)
        {
            logger.Error($"Failed to restart: {e}");
        }
    }
}