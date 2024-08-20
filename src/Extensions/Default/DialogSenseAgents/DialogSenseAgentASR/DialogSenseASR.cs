using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using ACAT.Lib.Core.DialogSenseManagement;
using ACAT.Lib.Core.Utility;
using System.Globalization;
using System.Runtime.InteropServices;

namespace ACAT.Extensions.Default.DialogSenseAgents
{
    [DescriptorAttribute("29C5BB4C-1092-41F4-9291-2A582C2E53DC",
                        "Dialog Sense extension for ASR",
                        "Interface to the ASR backend to convert speech to text")]
    public class DialogSenseASR : IDialogSenseAgent
    {
        private ClientWebSocket client;

        private const string Url = "ws://127.0.0.1:5000/ws/acat";

        private DialogTranscript _dialogTranscript;

        public event MessageReceivedEventHandler EvtMessageReceived;

        public DialogSenseASR()
        {
            _dialogTranscript = new DialogTranscript();
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        public DialogTranscript DialogTranscript => _dialogTranscript;

        public DialogSenseSource GetDialogSenseSource => DialogSenseSource.ASR;

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

                EvtMessageReceived?.Invoke(this, jsonResponse);
            }
        }

        public bool IsConnected()
        {
            return client != null && client.State == WebSocketState.Open;
        }

        public bool Init(CultureInfo ci)
        {
            return true;
        }

        public bool LoadDefaultSettings()
        {
            return true;
        }

        public bool LoadSettings(string configFileDirectory)
        {
            return true;

        }

        
        public bool SaveSettings(string configFileDirectory)
        {
            return true;
        }

        public void Uninit()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}

