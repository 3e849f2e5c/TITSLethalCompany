using System.Text;
using NativeWebSocket;
using Newtonsoft.Json;

namespace TITSLethalCompany.TITSApi;

public class TITSWebSocket
{
    private static string eventsWebSocketUri => $"ws://localhost:{ConfigManager.TITSPort}/events";
    private WebSocket? eventsClient;
    
    public async void ConnectWebSockets()
    {
        // TODO reconnect logic
        Close();
        eventsClient = WebSocketFactory.CreateInstance(eventsWebSocketUri);
        eventsClient.OnOpen += () =>
        {
            Utils.LogInfo("TITS WebSocket Connected!");
        };

        eventsClient.OnError += e =>
        {
            Utils.LogError($"TITS WebSocket Error: {e}");
        };

        eventsClient.OnClose += e =>
        {
            Utils.LogInfo($"TITS WebSocket Disconnected: {e}");
        };

        eventsClient.OnMessage += msg =>
        {
            var text = Encoding.UTF8.GetString(msg);

            TITSApiResponse response = JsonConvert.DeserializeObject<TITSApiResponse>(text);

            switch (response.messageType)
            {
                case nameof(TITSHitEvent):
                    var hitEvent = JsonConvert.DeserializeObject<TITSHitEvent>(text);
                    Plugin.OnItemHit(hitEvent.data);
                    break;
                case nameof(TITSApiError):
                    var errorEvent = JsonConvert.DeserializeObject<TITSApiError>(text);
                    Utils.LogError($"Received TITSAPIError: {errorEvent.data.message}");
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