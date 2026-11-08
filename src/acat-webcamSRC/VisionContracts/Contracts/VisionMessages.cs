using System.Runtime.Serialization;

namespace VisionContracts.Contracts
{
    [DataContract]
    public enum VisionHostCommandType
    {
        [EnumMember]
        StartVision,
        [EnumMember]
        StopVision,
        [EnumMember]
        ShutdownHost,
        [EnumMember]
        NativeCommand,
        [EnumMember]
        ShowVideo,
        [EnumMember]
        HideVideo
    }

    [DataContract]
    public enum VisionHostEventType
    {
        [EnumMember]
        HostReady,
        [EnumMember]
        HostStopped,
        [EnumMember]
        NativeEvent,
        [EnumMember]
        HostError,
        [EnumMember]
        VisionStopped
    }

    [DataContract]
    public sealed class VisionHostCommand
    {
        [DataMember(Order = 1)]
        public VisionHostCommandType CommandType { get; set; }

        [DataMember(Order = 2)]
        public string Payload { get; set; }

        [DataMember(Order = 3)]
        public double Value { get; set; }

        [DataMember(Order = 4)]
        public string CorrelationId { get; set; }
    }

    [DataContract]
    public sealed class VisionHostEvent
    {
        [DataMember(Order = 1)]
        public VisionHostEventType EventType { get; set; }

        [DataMember(Order = 2)]
        public string Payload { get; set; }

        [DataMember(Order = 3)]
        public string CorrelationId { get; set; }
    }
}
