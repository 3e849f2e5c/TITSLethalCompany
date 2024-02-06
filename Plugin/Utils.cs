using System;
using System.Collections.Generic;

namespace TITSLethalCompany;

public static class Utils
{
    public static Item? FindItemByName(string itemName)
    {
        StartOfRound? startOfRound = StartOfRound.Instance;
        if (startOfRound == null) { return null; }

        AllItemsList allItemList = startOfRound.allItemsList;
        if (allItemList == null) { return null; }

        List<Item>? itemList = allItemList.itemsList;
        if (itemList == null) { return null; }

        foreach (var item in itemList)
        {
            if (!item.itemName.Equals(itemName, StringComparison.OrdinalIgnoreCase)) { continue; }
            return item;
        }

        return null;
    }
    
    public static void LogInfo(string msg)
        => Plugin.StaticLogger.LogInfo($"{Plugin.PLUGIN_NAME}: {msg}");
    
    public static void LogError(string msg)
        => Plugin.StaticLogger.LogError($"{Plugin.PLUGIN_NAME}: {msg}");
}