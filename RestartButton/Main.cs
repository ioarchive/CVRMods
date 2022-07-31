using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using ABI_RC.Core;
using ABI_RC.Core.InteractionSystem;
using MelonLoader;
using RestartButton;
using UnityEngine;
using Main = RestartButton.Main;

[assembly: MelonInfo(typeof(Main), Guh.Name, Guh.Version, Guh.Author, Guh.DownloadLink)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]

namespace RestartButton;

public static class Guh
{
    public const string Name = "RestartButton";
    public const string Author = "Animal";
    public const string Version = "1.0.0";
    public const string DownloadLink = "https://github.com/Aniiiiiimal/CVRMods";
}

public class Main : MelonMod
{
    public override void OnUpdate()
    {
        if (!Input.GetKeyDown(KeyCode.End)) return;
        try
        {
            new Thread(() =>
            {
                LoggerInstance.Warning("Restarting!");
                RootLogic.Instance.QuitApplication();
                Thread.Sleep(420);
                Process.Start($"{Environment.CurrentDirectory}/ChilloutVR.exe",
                    Environment.GetCommandLineArgs().ToString());
                // circumvent "Another instance is already running"
            }).Start();   
            /*Process.Start("cmd.exe", "/C taskkill /im ChilloutVR.exe");*/
        } catch (Exception e)
        {
            LoggerInstance.Error($"Failed to restart: {e}");
        }
    }
}