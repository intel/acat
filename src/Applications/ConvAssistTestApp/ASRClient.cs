using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Security.Policy;

namespace ACAT.Applications.ConvAssistTestApp
{
    internal class ASRClient
    {
        private ClientWebSocket client;

        private const string Url = "ws://127.0.0.1:5000/ws/acat";

        public delegate void MessageReceivedEventHandler(object sender, ASRResponse e);

        public event MessageReceivedEventHandler EvtMessageReceived;

        public delegate void RawMessageReceivedEventHandler(object sender, ASRResponseRaw e); 

        public event RawMessageReceivedEventHandler EvtRawMessageReceived;

        public ASRClient()
        {
            
        }

        public async Task ConnectAsync()
        {
            if (client != null && client.State == WebSocketState.Open)
            {
                return;
            }

            client = new ClientWebSocket();


            Uri serverUri = new Uri(Url);
            await client.ConnectAsync(serverUri, CancellationToken.None);

            Task.Run(() => ReceiveMessagesAsync());
        }

        public void Disconnect()
        {
            if (client == null)
            {
                return;
            }
            client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None).GetAwaiter().GetResult();
            client.Dispose();
            client = null;
        }

        public async Task SendAsync(String jsonMessage)
        {
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage));
            await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task ReceiveMessagesAsync()
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);

            while (client.State == WebSocketState.Open)
            {
                Console.WriteLine("Waiting...");
                WebSocketReceiveResult result = await client.ReceiveAsync(buffer, CancellationToken.None);
                string jsonResponse = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                //Console.WriteLine("Received: " + jsonResponse);

                EvtRawMessageReceived?.Invoke(this, new ASRResponseRaw(jsonResponse));
                
                if (!jsonResponse.Contains("ACAT VAD ASR"))
                {
                    var receivedMessage = JsonSerializer.Deserialize<ASRResponse>(jsonResponse);
                    EvtMessageReceived?.Invoke(this, receivedMessage);  
                    Console.WriteLine("ASR: " + receivedMessage.text);
                }
                else
                {
                    Console.WriteLine("Received: " + jsonResponse);
                }
            }
        }

        public bool IsConnected()
        {
            return client != null && client.State == WebSocketState.Open;
        }
    }
}
