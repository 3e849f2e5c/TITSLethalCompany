using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TITSLethalCompany.Networking;
using TITSLethalCompany.TITSApi;
using Unity.Netcode;
using UnityEngine;

namespace TITSLethalCompany
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_NAME = "TITSLethalCompany";
        private const string PLUGIN_GUID = "com.kosblue.TITSLethalCompany";
        private const string PLUGIN_VERSION = "1.0.0";

        public static Plugin? Instance;
        public static ManualLogSource StaticLogger = null!;
        private readonly TITSWebSocket webSocket = new();

        public static void OnItemHit(TITSHitResponseData data)
        {
            string itemName = data.itemName;
            Utils.LogInfo($"Threw item: {itemName}");

            bool itemFound = Utils.FindItemByName(itemName) is not null;

            if (GameNetworkManager.Instance is not { localPlayerController: { } controller }) { return; }

            bool damagePlayer = itemFound || ConfigManager.DamageFromAnyItem;

            if (ConfigManager.ItemDamage > 0 && damagePlayer)
            {
                controller.DamagePlayer(ConfigManager.ItemDamage, true, true, CauseOfDeath.Mauling);
            }

            if (ConfigManager.SpawnItems && itemFound)
            {
                NetworkHandler.NetSpawnItemOnPlayer(itemName, controller.transform.position, controller.isInElevator);
            }
        }

        public static void SpawnItemOnPlayer(string itemName, Vector3 pos, bool inElevator)
        {
            Transform parent = inElevator
                ? StartOfRound.Instance.elevatorTransform
                : StartOfRound.Instance.propsContainer;

            Item? matchingItem = Utils.FindItemByName(itemName);

            if (matchingItem is null)
            {
                Utils.LogError($"No matching item found for {itemName}");
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
            Instance = this;
            Logger.LogInfo($"{PLUGIN_NAME} {PLUGIN_VERSION} loaded");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }
}