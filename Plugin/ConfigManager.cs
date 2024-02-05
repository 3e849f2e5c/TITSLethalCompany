using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Configuration;

namespace TITSLethalCompany;

public static class ConfigManager
{
    public static string Port = "42069";
    public static int ItemDamage = 10;
    public static bool PullPinOnFlashBang = true;

    private static readonly string configPath = Path.Combine(Paths.ConfigPath, "TITSLethalCompany.cfg");
    private static ConfigFile? configFile;

    public static void LoadConfig()
    {
        EnsureConfigExists();
        if (configFile is null) { return; }

        Port = GetConfigValue("TITSPort", Port, "What port to use for TITS API.");
        ItemDamage = GetConfigValue("ItemDamage", ItemDamage, "How much damage should throwing items deal to the player.");
        //PullPinOnFlashBang = GetConfigValue("PullPinOnFlashBang", true, "If a flashbang is thrown at the player should the pin be automatically pulled.");
        return;

        static T GetConfigValue<T>(string key, T defaultValue, string? description = null)
        {
            return configFile!.Bind("Settings", key, defaultValue, description).Value;
        }
    }

    private static void EnsureConfigExists()
    {
        if (configFile is not null) { return; }

        if (!File.Exists(configPath))
        {
            using StreamWriter streamWriter = File.CreateText(configPath);
        }

        configFile = new ConfigFile(configPath, true);
    }
}