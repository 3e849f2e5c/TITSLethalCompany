using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LC_API.Networking;
using LC_API.Networking.Serializers;
using TITSLethalCompany.Networking;
using TITSLethalCompany.TITSApi;
using Unity.Netcode;
using UnityEngine;

namespace TITSLethalCompany
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource StaticLogger = null!;
        private readonly TITSWebSocket webSocket = new();

        public static void OnItemHit(TITSHitResponseData data)
        {
            string itemName = data.itemName;
            StaticLogger.LogInfo($"{PluginInfo.PLUGIN_NAME} {itemName}");

            if (GameNetworkManager.Instance is not { localPlayerController: { } controller }) { return; }

            if (ConfigManager.ItemDamage > 0)
            {
                controller.DamagePlayer(ConfigManager.ItemDamage, true, true, CauseOfDeath.Mauling);
            }

            if (!ConfigManager.SpawnItems) { return; }
            NetworkHandler.NetSpawnItemOnPlayer(itemName, controller.transform.position, controller.isInElevator);
        }

        public static void SpawnItemOnPlayer(string itemName, Vector3 pos, bool inElevator)
        {
            Transform parent = inElevator
                ? StartOfRound.Instance.elevatorTransform
                : StartOfRound.Instance.propsContainer;

            List<Item>? itemList = StartOfRound.Instance.allItemsList.itemsList;
            Item? matchingItem = itemList?.FirstOrDefault(item => item.itemName.Equals(itemName, StringComparison.OrdinalIgnoreCase));

            if (matchingItem is null)
            {
                StaticLogger.LogError($"No matching item found for {itemName}");
                StaticLogger.LogError($"{string.Join(",", StartOfRound.Instance.allItemsList.itemsList.Select(item => item.itemName))}");
                return;
            }

            GameObject go = Instantiate(matchingItem.spawnPrefab, pos, Quaternion.identity, parent);

            if (go.GetComponent<GrabbableObject>() is { } grabbable)
            {
                grabbable.SetScrapValue(0);
                grabbable.fallTime = 0f;
            }

            go.GetComponent<NetworkObject>()?.Spawn();

            if (ConfigManager.PullPinOnStunGrenade && go.GetComponent<StunGrenadeItem>() is { } stunGrenadeItem)
            {
                NetworkHandler.NetPullStunGrenadePin(stunGrenadeItem);
            }
        }

        public void OnTimeOfDayAwake()
            => webSocket.ConnectWebSockets();

        public void TimeOfDayUpdate()
            => webSocket.Update();

        public void Awake()
        {
            ConfigManager.LoadConfig();
            NetworkHandler.Register();
            StaticLogger = Logger;
            TimeOfDayPatch.Plugin = this;
            Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION} loaded");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(TimeOfDay))]
    [SuppressMessage("ReSharper", "Unity.NoNullPropagation")]
    public class TimeOfDayPatch
    {
        public static Plugin? Plugin;

        [HarmonyPrefix, HarmonyPatch("Update")]
        public static void Update(ref TimeOfDay __instance)
            => Plugin?.TimeOfDayUpdate();

        [HarmonyPrefix, HarmonyPatch("Awake")]
        public static void Awake(ref TimeOfDay __instance)
            => Plugin?.OnTimeOfDayAwake();
    }
}