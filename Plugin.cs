using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace TITSLethalCompany
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        // FIXME this could be dynamically retrieved probably to make it future proof
        private enum LethalCompanyItems
        {
            Boombox = 1,
            Flashlight = 3,
            Jetpack = 4,
            LockPicker = 6,
            ProFlashlight = 9,
            Shovel = 10,
            StunGrenade = 11,
            ExtensionLadder = 12,
            TZPInhalant = 13,
            WalkieTalkie = 14,
            ZapGun = 15,
            Airhorn = 17
        }

        private readonly byte[] buffer = new byte[2048];
        private Socket? client;

        private DateTimeOffset nextConnectionAttempt = DateTimeOffset.MinValue;

        public async void Update()
        {
            if (client is not { Connected: true } socket)
            {
                Logger.LogInfo("Not connected, attempting to connect...");
                if (DateTimeOffset.UtcNow < nextConnectionAttempt) { return; }

                try
                {
                    client = new(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                    client.Connect(IPAddress.IPv6Loopback, 42068);
                    nextConnectionAttempt = DateTimeOffset.UtcNow.AddSeconds(5);
                }
                catch (Exception e)
                {
                    Logger.LogError($"Could not connect to websocket, is the server running?:\n{e}");
                    throw;
                }
                return;
            }

            int bytesRead = await socket.ReceiveAsync(buffer, SocketFlags.None);
            if (bytesRead is 0) { return; }

            string data = Encoding.UTF8.GetString(buffer[..bytesRead]);
            
            if (data == "send nudes pls")
            {
                socket.Send("8==D"u8.ToArray());
                return;
            }
            
            Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} {data}");

            if (GameNetworkManager.Instance?.localPlayerController is { } controller)
            {
                controller.DamagePlayer(10, true, true, CauseOfDeath.Kicking);

                LethalCompanyItems id = LethalCompanyItems.Airhorn;
                switch (data)
                {
                    case "lc_airhorn":
                        id = LethalCompanyItems.Airhorn;
                        break;
                    case "lc_shovel":
                        id = LethalCompanyItems.Shovel;
                        break;
                    case "lc_flashlight":
                        id = LethalCompanyItems.Flashlight;
                        break;
                    case "lc_proflashlight":
                        id = LethalCompanyItems.ProFlashlight;
                        break;
                    case "lc_stungrenade":
                        id = LethalCompanyItems.StunGrenade;
                        break;
                    case "lc_extensionladder":
                        id = LethalCompanyItems.ExtensionLadder;
                        break;
                    case "lc_tzpinhalant":
                        id = LethalCompanyItems.TZPInhalant;
                        break;
                    case "lc_walkietalkie":
                        id = LethalCompanyItems.WalkieTalkie;
                        break;
                    case "lc_zapgun":
                        id = LethalCompanyItems.ZapGun;
                        break;
                    case "lc_jetpack":
                        id = LethalCompanyItems.Jetpack;
                        break;
                    case "lc_boombox":
                        id = LethalCompanyItems.Boombox;
                        break;
                    case "lc_lockpicker":
                        id = LethalCompanyItems.LockPicker;
                        break;
                }

                float reverseRotation = controller.transform.rotation.eulerAngles.y + 180f;
                var reverseQuaternion = Quaternion.Euler(0, reverseRotation, 0);

                Transform parent = controller.isInElevator
                    ? StartOfRound.Instance.elevatorTransform
                    : StartOfRound.Instance.propsContainer;

                GameObject go = Instantiate(
                    StartOfRound.Instance.allItemsList.itemsList[(int)id].spawnPrefab,
                    controller.gameObject.transform.position,
                    reverseQuaternion,
                    parent);

                if (go.GetComponent<GrabbableObject>() is { } grabbable)
                {
                    grabbable.SetScrapValue(0);
                    grabbable.fallTime = 0f;
                }

                if (go.GetComponent<StunGrenadeItem>() is { } stunGrenadeItem)
                {
                    stunGrenadeItem.pinPulled = true;
                }

                if (go.GetComponent<ExtensionLadderItem>() is { } ladder)
                {
                    Vector3 posBehind = controller.transform.position - controller.transform.forward * 5;
                    go.transform.SetPositionAndRotation(posBehind, reverseQuaternion);
                    ladder.ItemActivate(true);
                }
                go.GetComponent<NetworkObject>()?.Spawn();
            }
        }

        public void Awake()
        {
            TimeOfDayPatch.Plugin = this;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        public void OnDestroy()
        {
            Logger.LogInfo("Destroying plugin...");
            client?.Shutdown(SocketShutdown.Both);
            client?.Close();
        }
    }

    [HarmonyPatch(typeof(TimeOfDay))]
    public class TimeOfDayPatch
    {
        public static Plugin? Plugin;
        [HarmonyPrefix, HarmonyPatch("Update")]
        public static void Update(ref TimeOfDay __instance)
        {
            Plugin?.Update();
        }
    }
}
