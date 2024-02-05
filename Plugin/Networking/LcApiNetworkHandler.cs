using LC_API.Networking;
using UnityEngine;

namespace TITSLethalCompany.Networking;

public static class LcApiNetworkHandler
{
    private const string ItemHitNetworkMessage = "TITSItemHit";
    private const string FlashBangPullPinNetworkMessage = "TITSFlashBangPullPin";

    public static void Register()
        => Network.RegisterAll();

    [NetworkMessage(ItemHitNetworkMessage, relayToSelf: true)]
    public static void TITSItemHitHandler(ulong sender, NetSpawnItemOnPlayer message)
    {
        if (GameNetworkManager.Instance is not { localPlayerController: { } controller }) { return; }

        if (!controller.IsHost) { return; }

        Plugin.SpawnItemOnPlayer(message.ItemName, message.Position, message.InElevator);
    }

    [NetworkMessage(FlashBangPullPinNetworkMessage, relayToSelf: true)]
    public static void TITSFlashBangPullPinHandler(ulong sender, NetPullFlashBangPin message)
    {
        if (!LC_API.GameInterfaceAPI.Features.Item.TryGet(message.NetworkId, out var item)) { return; }

        if (item.GetComponent<StunGrenadeItem>() is { } stunGrenadeItem) { stunGrenadeItem.pinPulled = true; }
    }

    public static void NetSpawnItemOnPlayer(string itemName, Vector3 pos, bool inElevator)
        => Network.Broadcast(ItemHitNetworkMessage, new NetSpawnItemOnPlayer
        {
            ItemName = itemName,
            Position = pos,
            InElevator = inElevator
        });

    public static void NetPullStunGrenadePin(StunGrenadeItem item)
        => Network.Broadcast(FlashBangPullPinNetworkMessage, new NetPullFlashBangPin
        {
            NetworkId = item.NetworkObjectId
        });
}