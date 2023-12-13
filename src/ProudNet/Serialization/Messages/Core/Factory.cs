﻿namespace ProudNet.Serialization.Messages.Core
{
    internal class CoreMessageFactory : MessageFactory<ProudCoreOpCode, ICoreMessage>
    {
        public static CoreMessageFactory Default { get; } = new CoreMessageFactory();

        public CoreMessageFactory()
        {
            // S2C
            Register<ConnectServerTimedoutMessage>(ProudCoreOpCode.ConnectServerTimedout);
            Register<NotifyServerConnectionHintMessage>(ProudCoreOpCode.NotifyServerConnectionHint);
            Register<NotifyCSSessionKeySuccessMessage>(ProudCoreOpCode.NotifyCSSessionKeySuccess);
            Register<NotifyProtocolVersionMismatchMessage>(ProudCoreOpCode.NotifyProtocolVersionMismatch);
            Register<NotifyServerDeniedConnectionMessage>(ProudCoreOpCode.NotifyServerDeniedConnection);
            Register<NotifyServerConnectSuccessMessage>(ProudCoreOpCode.NotifyServerConnectSuccess);
            Register<RequestStartServerHolepunchMessage>(ProudCoreOpCode.RequestStartServerHolepunch);
            Register<ServerHolepunchAckMessage>(ProudCoreOpCode.ServerHolepunchAck);
            Register<NotifyClientServerUdpMatchedMessage>(ProudCoreOpCode.NotifyClientServerUdpMatched);
            Register<PeerUdp_ServerHolepunchAckMessage>(ProudCoreOpCode.PeerUdp_ServerHolepunchAck);
            Register<UnreliablePongMessage>(ProudCoreOpCode.UnreliablePong);
            Register<ReliableRelay2Message>(ProudCoreOpCode.ReliableRelay2);
            Register<UnreliableRelay2Message>(ProudCoreOpCode.UnreliableRelay2);

            // C2S
            Register<NotifyCSEncryptedSessionKeyMessage>(ProudCoreOpCode.NotifyCSEncryptedSessionKey);
            Register<NotifyServerConnectionRequestDataMessage>(ProudCoreOpCode.NotifyServerConnectionRequestData);
            Register<ServerHolepunchMessage>(ProudCoreOpCode.ServerHolepunch);
            Register<NotifyHolepunchSuccessMessage>(ProudCoreOpCode.NotifyHolepunchSuccess);
            Register<PeerUdp_ServerHolepunchMessage>(ProudCoreOpCode.PeerUdp_ServerHolepunch);
            Register<PeerUdp_NotifyHolepunchSuccessMessage>(ProudCoreOpCode.PeerUdp_NotifyHolepunchSuccess);
            Register<UnreliablePingMessage>(ProudCoreOpCode.UnreliablePing);
            Register<SpeedHackDetectorPingMessage>(ProudCoreOpCode.SpeedHackDetectorPing);
            Register<ReliableRelay1Message>(ProudCoreOpCode.ReliableRelay1);
            Register<UnreliableRelay1Message>(ProudCoreOpCode.UnreliableRelay1);
            Register<UnreliableRelay1_RelayDestListCompressedMessage>(ProudCoreOpCode.UnreliableRelay1_RelayDestListCompressed);

            // SC
            Register<RmiMessage>(ProudCoreOpCode.Rmi);
            Register<EncryptedReliableMessage>(ProudCoreOpCode.EncryptedReliable);
            Register<Encrypted_UnReliableMessage>(ProudCoreOpCode.Encrypted_UnReliable);
            Register<CompressedMessage>(ProudCoreOpCode.Compressed);
            Register<ReliableUdp_FrameMessage>(ProudCoreOpCode.ReliableUdp_Frame);
        }
    }
}
