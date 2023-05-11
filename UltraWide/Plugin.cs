using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using PugRP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UltraWide;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BasePlugin
{
    private const string PluginGuid = "p1xel8ted.corekeeper.ultrawide";
    private const string PluginName = "CoreKeeper UltraWide";
    private const string PluginVersion = "0.0.1";

    private const string LeftPillarBoxName = "Left";
    private const string RightPillarBoxName = "Right";
    private static ConfigEntry<bool> UseCustomMaxWidth { get; set; }
    private static ConfigEntry<int> CustomMaxWidth { get; set; }

    public override void Load()
    {
        UseCustomMaxWidth = Config.Bind("General", "UseCustomMaxWidth", false, "At auto, you will see the world spawning/de-spawning. It's not so bad at 21:9, but will be worse at wider resolutions. Adjust this value to your liking.");
        UseCustomMaxWidth.SettingChanged += (_, _) =>
        {
            if (UseCustomMaxWidth.Value)
            {
                UpdatePugCameras(true);
            }
            else
            {
                UpdatePugCameras();
            }
        };
        CustomMaxWidth = Config.Bind("General", "MaxWidth", 480, "This is not the width in screen pixels. For example at 3440x1440 (21:9), a value of 630 expands the viewport entirely.");
        CustomMaxWidth.SettingChanged += (_, _) =>
        {
            if (UseCustomMaxWidth.Value)
            {
                UpdatePugCameras(true);
            }
        };
        SceneManager.sceneLoaded += (UnityAction<Scene, LoadSceneMode>) SceneManagerOnSceneLoaded;
    }

    public override bool Unload()
    {
        SceneManager.sceneLoaded -= (UnityAction<Scene, LoadSceneMode>) SceneManagerOnSceneLoaded;
        return base.Unload();
    }

    private static void UpdatePugCameras(bool custom = false)
    {
        var pugCameras = Resources.FindObjectsOfTypeAll<PugCamera>();
        foreach (var cam in pugCameras)
        {
            cam.m_outputWidth = custom ? CustomMaxWidth.Value : cam.m_maxOutputWidth;
            if (cam.camera != null)
            {
                cam.camera.aspect = Display.main.systemWidth / (float) Display.main.systemHeight;
            }
        }
    }

    private static void RemovePillars()
    {
        var spr = Resources.FindObjectsOfTypeAll<SpriteRenderer>();
        foreach (var s in spr)
        {
            if (s.name == LeftPillarBoxName)
            {
                s.gameObject.SetActive(false);
            }

            if (s.name == RightPillarBoxName)
            {
                s.gameObject.SetActive(false);
            }
        }
    }

    private static void UpdateCameras()
    {
        var cameras = Resources.FindObjectsOfTypeAll<Camera>();
        foreach (var cam in cameras)
        {
            cam.aspect = Display.main.systemWidth / (float) Display.main.systemHeight;
        }
    }

    private static void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        UpdateCameras();
        UpdatePugCameras();
        RemovePillars();
    }
}