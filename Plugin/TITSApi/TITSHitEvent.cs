using System;

namespace TITSLethalCompany.TITSApi;

/// <summary>
/// This Event gets broadcast whenever the player gets hit!
/// </summary>
public record struct TITSHitEvent(
    long timestamp,
    string requestID,
    string apiName,
    Version apiVersion,
    string messageType,
    TITSHitResponseData data);

public record struct TITSHitResponseData(
    Guid itemId,
    string itemName,
    Guid triggerId,
    string triggerName,
    float strength,
    HitVector3 direction);

public record struct HitVector3(float x, float y, float z);