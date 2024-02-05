using System.Text;
using NativeWebSocket;
using Newtonsoft.Json;

namespace TITSLethalCompany.TITSApi;

public class TITSWebSocket
{
    private static string eventsWebSocketUri => $"ws://localhost:{ConfigManager.Port}/events";
    private WebSocket? eventsClient;
    
    public async void ConnectWebSockets()
    {
        // TODO reconnect logic
        Close();
        eventsClient = WebSocketFactory.CreateInstance(eventsWebSocketUri);
        eventsClient.OnOpen += () =>
        {
            Plugin.StaticLogger.LogInfo("TITS WebSocket Connected!");
        };

        eventsClient.OnError += e =>
        {
            Plugin.StaticLogger.LogError($"TITS WebSocket Error: {e}");
        };

        eventsClient.OnClose += e =>
        {
            Plugin.StaticLogger.LogInfo($"TITS WebSocket Disconnected: {e}");
        };

        eventsClient.OnMessage += msg =>
        {
            var text = Encoding.UTF8.GetString(msg);

            Plugin.StaticLogger.LogInfo($"{text}");
            TITSApiResponse response = JsonConvert.DeserializeObject<TITSApiResponse>(text);

            switch (response.messageType)
            {
                case "TITSHitEvent":
                    var hitEvent = JsonConvert.DeserializeObject<TITSHitEvent>(text);
                    Plugin.OnItemHit(hitEvent.data);
                    break;
                case "APIError":
                    var errorEvent = JsonConvert.DeserializeObject<TITSApiError>(text);
                    Plugin.StaticLogger.LogError($"Received TITSAPIError: {errorEvent.data.message}");
                    break;
            }
        };

        await eventsClient.Connect();
    }

    private async void Close()
    {
        if (eventsClient is null) { return; }
        await eventsClient.Close();
    }

    public void Update()
        => eventsClient?.DispatchMessageQueue();
}