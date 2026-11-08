using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace VisionContracts.Contracts
{
    public static class VisionPipeProtocol
    {
        public static string SerializeCommand(VisionHostCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return Serialize(command);
        }

        public static VisionHostCommand DeserializeCommand(string payload)
        {
            return Deserialize<VisionHostCommand>(payload);
        }

        public static string SerializeEvent(VisionHostEvent hostEvent)
        {
            if (hostEvent == null)
            {
                throw new ArgumentNullException(nameof(hostEvent));
            }

            return Serialize(hostEvent);
        }

        public static VisionHostEvent DeserializeEvent(string payload)
        {
            return Deserialize<VisionHostEvent>(payload);
        }

        private static string Serialize<T>(T value)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, value);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private static T Deserialize<T>(string payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                throw new ArgumentException("Payload is empty.", nameof(payload));
            }

            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)))
            {
                return (T)serializer.ReadObject(stream);
            }
        }
    }
}
