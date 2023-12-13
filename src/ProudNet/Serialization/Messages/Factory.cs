﻿namespace ProudNet.Serialization.Messages
{
    internal class RmiMessageFactory : MessageFactory<ProudOpCode, IMessage>
    {
        public static RmiMessageFactory Default { get; } = new RmiMessageFactory();

        public RmiMessageFactory()
        {
            // C2S
            Register<ReliablePingMessage>(ProudOpCode.ReliablePing);
            Register<P2P_NotifyDirectP2PDisconnectedMessage>(ProudOpCode.P2P_NotifyDirectP2PDisconnected);
            Register<NotifyUdpToTcpFallbackByClientMessage>(ProudOpCode.NotifyUdpToTcpFallbackByClient);
            Register<P2PGroup_MemberJoin_AckMessage>(ProudOpCode.P2PGroup_MemberJoin_Ack);
            Register<NotifyP2PHolepunchSuccessMessage>(ProudOpCode.NotifyP2PHolepunchSuccess);
            Register<ShutdownTcpMessage>(ProudOpCode.ShutdownTcp);
            Register<NotifyLogMessage>(ProudOpCode.NotifyLog);
            Register<NotifyJitDirectP2PTriggeredMessage>(ProudOpCode.NotifyJitDirectP2PTriggered);
            Register<NotifyNatDeviceNameDetectedMessage>(ProudOpCode.NotifyNatDeviceNameDetected);
            Register<C2S_RequestCreateUdpSocketMessage>(ProudOpCode.C2S_RequestCreateUdpSocket);
            Register<C2S_CreateUdpSocketAckMessage>(ProudOpCode.C2S_CreateUdpSocketAck);
            Register<ReportC2CUdpMessageCountMessage>(ProudOpCode.ReportC2CUdpMessageCount);
            Register<ReportC2SUdpMessageTrialCountMessage>(ProudOpCode.ReportC2SUdpMessageTrialCount);

            // S2C
            Register<ReliablePongMessage>(ProudOpCode.ReliablePong);
            Register<NotifyUdpToTcpFallbackByServerMessage>(ProudOpCode.NotifyUdpToTcpFallbackByServer);
            Register<ShutdownTcpAckMessage>(ProudOpCode.ShutdownTcpAck);
            Register<P2PGroup_MemberJoinMessage>(ProudOpCode.P2PGroup_MemberJoin);
            Register<P2PGroup_MemberJoin_UnencryptedMessage>(ProudOpCode.P2PGroup_MemberJoin_Unencrypted);
            Register<P2PRecycleCompleteMessage>(ProudOpCode.P2PRecycleComplete);
            Register<RequestP2PHolepunchMessage>(ProudOpCode.RequestP2PHolepunch);
            Register<P2P_NotifyDirectP2PDisconnected2Message>(ProudOpCode.P2P_NotifyDirectP2PDisconnected2);
            Register<P2PGroup_MemberLeaveMessage>(ProudOpCode.P2PGroup_MemberLeave);
            Register<NotifyDirectP2PEstablishMessage>(ProudOpCode.NotifyDirectP2PEstablish);
            Register<RenewP2PConnectionStateMessage>(ProudOpCode.RenewP2PConnectionState);
            Register<NewDirectP2PConnectionMessage>(ProudOpCode.NewDirectP2PConnection);
            Register<S2C_RequestCreateUdpSocketMessage>(ProudOpCode.S2C_RequestCreateUdpSocket);
            Register<S2C_CreateUdpSocketAckMessage>(ProudOpCode.S2C_CreateUdpSocketAck);
        }
    }
}
