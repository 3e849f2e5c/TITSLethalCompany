using System.IO;
using BepInEx;
using BepInEx.Configuration;

namespace TITSLethalCompany;

public static class ConfigManager
{
    public static string TITSPort = "42069";
    public static int ItemDamage = 10;
    public static bool PullPinOnStunGrenade = true;
    public static bool DamageFromAnyItem = false;
    public static bool SpawnItems = true;

    private static readonly string configPath = Path.Combine(Paths.ConfigPath, "TITSLethalCompany.cfg");
    private static ConfigFile? configFile;

    public static void LoadConfig()
    {
        EnsureConfigExists();
        if (configFile is null) { return; }

        TITSPort = GetConfigValue(nameof(TITSPort), TITSPort, "What port to use for TITS API.");
        ItemDamage = GetConfigValue(nameof(ItemDamage), ItemDamage, "How much damage should throwing items deal to the player. Set to 0 or lower to disable damage.");
        SpawnItems = GetConfigValue(nameof(SpawnItems), SpawnItems, "Set to false to disable spawning items completely.");
        PullPinOnStunGrenade = GetConfigValue(nameof(PullPinOnStunGrenade), PullPinOnStunGrenade, "If a stun grenade is thrown at the player should the pin be automatically pulled.");
        DamageFromAnyItem = GetConfigValue(nameof(DamageFromAnyItem), DamageFromAnyItem, "When set to false the damage will only be applied if the thrown item is a recognized Lethal Company item, set to true for all thrown items to cause damage.");
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