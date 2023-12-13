﻿using System.Net;
using BlubLib.Serialization;

namespace ProudNet.Serialization.Messages
{
    [BlubContract]
    internal class ReliablePingMessage : IMessage
    {
        [BlubMember(0)]
        public int RecentFrameRate { get; set; }
    }

    [BlubContract]
    internal class P2P_NotifyDirectP2PDisconnectedMessage : IMessage
    {
        [BlubMember(0)]
        public uint RemotePeerHostId { get; set; }

        [BlubMember(1)]
        public uint Reason { get; set; }
    }

    [BlubContract]
    internal class NotifyUdpToTcpFallbackByClientMessage : IMessage
    {
    }

    [BlubContract]
    internal class P2PGroup_MemberJoin_AckMessage : IMessage
    {
        [BlubMember(0)]
        public uint GroupHostId { get; set; }

        [BlubMember(1)]
        public uint AddedMemberHostId { get; set; }

        [BlubMember(2)]
        public uint EventId { get; set; }

        [BlubMember(3)]
        public bool LocalPortReuseSuccess { get; set; }
    }

    [BlubContract]
    internal class NotifyP2PHolepunchSuccessMessage : IMessage
    {
        [BlubMember(0)]
        public uint A { get; set; }

        [BlubMember(1)]
        public uint B { get; set; }

        [BlubMember(2)]
        public IPEndPoint ABSendAddr { get; set; }

        [BlubMember(3)]
        public IPEndPoint ABRecvAddr { get; set; }

        [BlubMember(4)]
        public IPEndPoint BASendAddr { get; set; }

        [BlubMember(5)]
        public IPEndPoint BARecvAddr { get; set; }
    }

    [BlubContract]
    internal class ShutdownTcpMessage : IMessage
    {
        [BlubMember(0)]
        public short Unk { get; set; }
    }

    [BlubContract]
    internal class NotifyLogMessage : IMessage
    {
        [BlubMember(0)]
        public TraceId TraceId { get; set; }

        [BlubMember(1)]
        public string Message { get; set; }
    }

    [BlubContract]
    internal class NotifyJitDirectP2PTriggeredMessage : IMessage
    {
        [BlubMember(0)]
        public uint HostId { get; set; }
    }

    [BlubContract]
    internal class NotifyNatDeviceNameDetectedMessage : IMessage
    {
        [BlubMember(0)]
        public string Name { get; set; }
    }

    [BlubContract]
    internal class C2S_RequestCreateUdpSocketMessage : IMessage
    {
    }

    [BlubContract]
    internal class C2S_CreateUdpSocketAckMessage : IMessage
    {
        [BlubMember(0)]
        public bool Success { get; set; }
    }

    [BlubContract]
    internal class ReportC2CUdpMessageCountMessage : IMessage
    {
        [BlubMember(0)]
        public uint HostId { get; set; }

        [BlubMember(1)]
        public uint UdpMessageTrialCount { get; set; }

        [BlubMember(2)]
        public uint UdpMessageSuccessCount { get; set; }
    }

    [BlubContract]
    internal class ReportC2SUdpMessageTrialCountMessage : IMessage
    {
        [BlubMember(0)]
        public int TrialCount { get; set; }
    }
}
