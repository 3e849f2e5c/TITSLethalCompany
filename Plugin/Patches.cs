using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace TITSLethalCompany;

// hook into the TimeOfDay class to get an update loop
[HarmonyPatch(typeof(TimeOfDay))]
[SuppressMessage("ReSharper", "Unity.NoNullPropagation")]
public class TimeOfDayPatch
{
    [HarmonyPrefix, HarmonyPatch("Update")]
    public static void Update(ref TimeOfDay __instance)
        => Plugin.Instance?.TimeOfDayUpdate();

    [HarmonyPrefix, HarmonyPatch("Awake")]
    public static void Awake(ref TimeOfDay __instance)
        => Plugin.Instance?.OnTimeOfDayAwake();
}