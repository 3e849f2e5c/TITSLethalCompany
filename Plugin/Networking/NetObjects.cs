using LC_API.Networking.Serializers;

namespace TITSLethalCompany.Networking;

public class NetPullFlashBangPin
{
    public ulong NetworkId { get; set; }
}

public class NetSpawnItemOnPlayer
{
    public string ItemName { get; set; } = string.Empty;
    public Vector3S Position { get; set; }
    public bool InElevator { get; set; }
}

