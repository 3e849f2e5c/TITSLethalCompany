using System;

namespace TITSLethalCompany.TITSApi;

public record struct TITSApiError(
    long timestamp,
    string requestID,
    string messageType,
    string apiName,
    Version apiVersion,
    TITSApiErrorData data);

public record struct TITSApiErrorData(
    int errorId,
    string message);