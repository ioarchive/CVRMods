using System;
using System.Diagnostics;
using System.IO;
using ABI_RC.Core;
using MelonLoader;
using RestartButton;
using UnityEngine;
using Main = RestartButton.Main;
using ButtonAPI = ChilloutButtonAPI.ChilloutButtonAPIMain;
using ABI_RC.Core.Player;
using System.Linq;
using System.Net;

[assembly: MelonInfo(typeof(Main), Guh.Name, Guh.Version, Guh.Author, Guh.DownloadLink)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]

namespace RestartButton;

public static class Guh
{
    public const string Name = "RestartButton";
    public const string Author = "Animal & Bluscream";
    public const string Version = "1.1.0";
    public const string DownloadLink = "https://github.com/Aniiiiiimal/CVRMods";
}

public class Main : MelonMod
{
    public const string bat_template = @"
taskkill /f /im {0}
timeout /t {1}
start """" {2}
";
    public MelonPreferences_Entry keybind;
    public MelonPreferences_Entry vr_failsave;

    public override void OnApplicationStart() {
        var cat = MelonPreferences.CreateCategory(Guh.Name);
        keybind = cat.CreateEntry<KeyCode>("restart_bind", KeyCode.End, "Restart Key Bind", "Key to press to restart game");
        vr_failsave = cat.CreateEntry<bool>("vr_failsave", false, "VR Failsave", "Failsave option to detect VR even if the command line arguments don't provide it");
        ButtonAPI.OnInit += () => {
            var menu = ButtonAPI.MainPage;
            menu.AddButton("Restart Game", "Restart ChilloutVR", () => {
                RestartGame();
            });
        };
    }

    public void RestartGame() {
        LoggerInstance.Warning("Restarting!");
        var filename = Process.GetCurrentProcess().ProcessName + ".exe";
        // var filepath = $"{Environment.CurrentDirectory}/{filename}";
        var args = string.Join(" ", Environment.GetCommandLineArgs());
        // MelonLogger.Warning(PlayerSetup.Instance._inVr ? "IN VR" : "NOT IN VR");
        // MelonLogger.Warning(PlayerSetup.Instance.vrHeadTracker.activeSelf ? "VR HEADSET ACTIVE" : "VR HEADSET INACTIVE");
        MelonLogger.Warning("Game start args: "+ args);
        if ((bool)vr_failsave.BoxedValue && PlayerSetup.Instance._inVr) args += " -vr";
        File.WriteAllText("restart.bat", string.Format(bat_template, filename, 3, args));
        Process.Start("restart.bat");
        RootLogic.Instance.QuitApplication();
        Environment.Exit(0);
    }

    public override void OnUpdate()
    {
        if (!Input.GetKeyDown((KeyCode)keybind.BoxedValue)) return;
        try
        {
            RestartGame();
            /*Process.Start("cmd.exe", "/C taskkill /im ChilloutVR.exe");*/
        } catch (Exception e)
        {
            LoggerInstance.Error($"Failed to restart: {e}");
        }
    }
}
