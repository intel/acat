using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ACAT.Lib.Core.DialogSenseManagement;
using ACAT.Lib.Core.Utility;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ACAT.Extensions.Default.DialogSenseAgents.DialogSenseAgentASR
{
    [DescriptorAttribute("29C5BB4C-1092-41F4-9291-2A582C2E53DC",
                        "Dialog Sense extension for ASR",
                        "Interface to the ASR backend to convert speech to text")]
    public class DialogSenseASR : IDialogSenseAgent
    {
        private ClientWebSocket client;

        private const string Url = "ws://127.0.0.1:5000/ws/acat";

        private DialogTranscript _dialogTranscript;

        public event JsonMessageReceivedEventHandler EvtJsonMessageReceived;

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

            try
            {
                while (client.State == WebSocketState.Open)
                {
                    Log.Debug("ASR:  Waiting for message...");
                    WebSocketReceiveResult result = await client.ReceiveAsync(buffer, CancellationToken.None);
                    string jsonResponse = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

                    if (!String.IsNullOrEmpty(jsonResponse))
                    {
                        Log.Debug("Received: " + jsonResponse);

                        EvtJsonMessageReceived?.Invoke(this, jsonResponse);

                        if (jsonResponse.Contains("ACAT VAD ASR"))
                        {
                            continue;
                        }

                        var receivedMessage = JsonConvert.DeserializeObject<ASRResponse>(jsonResponse);

                        Log.Debug("Received message from ASR: " + receivedMessage.text);
                        DialogTranscript.AddTurn(new DialogTurn(DialogTurnType.Other, receivedMessage.text));

                        EvtMessageReceived?.Invoke(this, receivedMessage.text);
                    }
                }

            }
            catch(Exception ex)
            {
                Log.Exception(ex);

                Log.Error("Error receiving message from ASR. " + ex.Message);
            }

            Log.Debug(Url + " is not connected.");
        }

        public bool IsConnected()
        {
            return client != null && client.State == WebSocketState.Open;
        }

        public bool Init(CultureInfo ci)
        {
            Task.Run(() => connectASR());

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
            if (IsConnected())
            {
                disconnectASR();
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }

        private bool connectASR()
        {
            if (IsConnected())
            {
                return true;
            }

            try
            {
                Log.Debug("Connecting to ASR...");
                ConnectAsync().GetAwaiter().GetResult();


                String model = "tiny.en";

                // MessageBox.Show(model);

                Thread.Sleep(2000);

                var msg = new StartMessage(true, 1, model, false);

                var json = JsonConvert.SerializeObject(msg);

                //var json = "{\"crg_on\":true,\"device_index\":1,\"model_size\":\"tiny.en\",\"save_audio\":false}";

                SendAsync(json).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var message = "Error connnecting to ASR. " + ex.Message;

                Log.Error(message);

                Log.Exception(ex);

                return false;
            }

            return true;
        }

        private bool disconnectASR()
        {
            if (!IsConnected())
            {
                return true;
            }

            try
            {
                
                Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error("Error disconnnecting from ASR. " + ex.ToString());
                return false;
            }

            return true;
        }
    }
}

