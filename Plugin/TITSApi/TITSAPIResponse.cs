using System;

namespace TITSLethalCompany.TITSApi;

public record struct TITSApiResponse(
    long timestamp,
    string requestID,
    string apiName,
    Version apiVersion,
    string messageType);