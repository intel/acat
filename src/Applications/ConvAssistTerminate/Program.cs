////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConvAssistTerminate
//
// Console application to send a request to ConvAssist to terminate gracefully
//
////////////////////////////////////////////////////////////////////////////
using ACAT.Lib.Core.Utility.NamedPipe;
using ACAT.Lib.Core.WordPredictionManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace ACAT.Applications.ConvAssistTerminate
{

    class Program
    {
        /// <summary>
        /// Name of ConvAssist Named Pipe
        /// </summary>
        private const String ConvAssistName = "ConvAssist";

        /// <summary>
        /// Exit code when it closes
        /// 0 - It closed ConvAssist
        /// 1 - There was no process (ConvAssist was not active)
        /// 2 - Error
        /// </summary>
        private static int _ExitCode = 2;

        /// <summary>
        /// End Task to display status of server connection?
        /// </summary>
        private static bool _FinishTask = false;

        /// <summary>
        /// The maximun time the program will wait for ConvAssist to close 
        /// </summary>
        private static int _MaxWaitTimeToCloseProgram = 20;

        private const String namedPipeName = "ACATConvAssistPipe"; 

        /// <summary>
        /// Main object of the Named Pipe server
        /// </summary>
        private static PipeServer _pipeServer = new PipeServer(namedPipeName, PipeDirection.InOut, true);
        /// <summary>
        /// Type of Exit codes when the app closses
        /// 0 - It closed ConvAssist
        /// 1 - There was no processes (ConvAssist was not active)
        /// 2 - Error
        /// </summary>
        public enum ExitCodes
        {
            ConvAssistClosed = 0,
            ConvAssistNotFound = 1,
            Error = 2
        }
        /// <summary>
        /// Type of messages send to ConvAssist
        /// </summary>
        public enum WordPredictorMessageTypes
        {
            ForceQuitApp = 10
        }
        /// <summary>
        /// Task to validate when the request to close ConvAssist the Processes are all done
        /// </summary>
        /// <returns></returns>
        private static async Task CheckConvAssistProcesses()
        {
            await Task.Delay(10);
            int counter = 0;
            bool finishTask = false;
            while (!finishTask)
            {
                if (counter == _MaxWaitTimeToCloseProgram)
                {
                    _ExitCode = (int)ExitCodes.Error;
                    Console.WriteLine("The program exceeds the max waiting time to close ConvAssist");
                    finishTask = true;
                }
                Process[] ConvAssistExeProcess = Process.GetProcessesByName(ConvAssistName);
                if (ConvAssistExeProcess.Length == 0)
                {
                    Console.WriteLine("ConvAssist processes are all closed");
                    _ExitCode = (int)ExitCodes.ConvAssistClosed;
                    finishTask = true;
                }
                counter += 1;
                await Task.Delay(1000);
            }
            _FinishTask = true;
        }

        static void Main(string[] args)
        {
            try
            {
                Process[] ConvAssistProcess = Process.GetProcessesByName(ConvAssistName);
                if (ConvAssistProcess.Length != 0)
                {
                    Console.WriteLine(" ");
                    Console.WriteLine("Starting Pipe Server");
                    Console.WriteLine(" ");
                    Console.WriteLine("Connecting to ConvAssist");
                    Console.WriteLine(" ");
                    _pipeServer.Start();
                    _ = NamePipeStatus().ConfigureAwait(false);
                    while (!_FinishTask)
                    {
                        Thread.Sleep(500);
                    }
                }
                else
                {
                    _ExitCode = (int)ExitCodes.ConvAssistNotFound;
                }
                Thread.Sleep(25);
            }
            catch (Exception ex)
            {
                _ExitCode = (int)ExitCodes.Error;
                Console.WriteLine("Exception: " + ex.Message);
            }
            finally
            {
                Environment.ExitCode = _ExitCode;
                Console.WriteLine("Closing App...");
            }
        }
        /// <summary>
        /// Task to Display in the console the status of the connection with ConvAssist
        /// </summary>
        /// <returns></returns>
        private static async Task NamePipeStatus()
        {
            await Task.Delay(10);
            int counter = 0;
            bool finishTask = false;
            bool sendMessage = true;
            while (!finishTask)
            {
                if (_pipeServer.ServerStream.IsConnected)
                {
                    Console.WriteLine("Connection Status: Successful");
                    counter = 0;
                    finishTask = true;
                }
                else
                {
                    Console.WriteLine("Waiting to connect to ConvAssist");
                }
                if (counter == _MaxWaitTimeToCloseProgram)
                {
                    _ExitCode = (int)ExitCodes.Error;
                    Console.WriteLine("The program exceeds the max waiting time to close ConvAssist");
                    finishTask = true;
                    sendMessage = false;
                    _FinishTask = true;
                }
                counter += 1;
                await Task.Delay(1000);
            }
            Console.WriteLine("Exiting Task: NamePipeStatus");
            if(sendMessage)
                SendMessage();
        }
        /// <summary>
        /// Sends a message to ConvAssist with the format that hold the request to quit the App
        /// </summary>
        private static void SendMessage()
        {
            try
            {
                Console.WriteLine("Sending Request to close ConvAssist");
                ConvAssistMessage message = new ConvAssistMessage(WordPredictorMessageTypes.ForceQuitApp, WordPredictionModes.None, "NA");
                string jsonMessage = JsonConvert.SerializeObject(message);
                _pipeServer.Send(jsonMessage);
            }
            catch (Exception es)
            {
                Console.WriteLine("Error sending Message to close ConvAssist: " + es.Message);
            }
            _ = CheckConvAssistProcesses();
        }
        [Serializable]
        internal class ConvAssistMessage
        {
            public String Data;
            public WordPredictorMessageTypes MessageType;
            public WordPredictionModes PredictionType;

            // this is the JSON representation of the data
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="msgType"></param>
            /// <param name="PredictionMode"></param>
            /// <param name="message"></param>
            public ConvAssistMessage(WordPredictorMessageTypes msgType, WordPredictionModes PredictionMode, String message)
            {
                MessageType = msgType;
                PredictionType = PredictionMode;
                Data = message;
            }
        }






    }




}
