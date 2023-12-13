using ProudNet.Serialization;

namespace Netsphere.Network.Message.P2P
{
    public interface IP2PMessage
    {
    }

    public class P2PMessageFactory : MessageFactory<P2POpCode, IP2PMessage>
    {
        public P2PMessageFactory()
        {
            Register<PlayerSpawnReqMessage>(P2POpCode.PlayerSpawnReq);
            Register<PlayerSpawnAckMessage>(P2POpCode.PlayerSpawnAck);
            Register<AbilitySyncMessage>(P2POpCode.AbilitySync);
            Register<EquippingItemSyncMessage>(P2POpCode.EquippingItemSync);
            Register<DamageInfoMessage>(P2POpCode.DamageInfo);
            Register<DamageRemoteInfoMessage>(P2POpCode.DamageRemoteInfo);
            Register<SnapShotMessage>(P2POpCode.SnapShot);
            Register<StateSyncMessage>(P2POpCode.StateSync);
            Register<BGEffectMessage>(P2POpCode.BGEffect);
            Register<DefensivePowerMessage>(P2POpCode.DefensivePower);
            Register<BlastObjectDestroyMessage>(P2POpCode.BlastObjectDestroy);
            Register<BlastObjectRespawnMessage>(P2POpCode.BlastObjectRespawn);
            Register<MindEnergyMessage>(P2POpCode.MindEnergy);
            Register<DamageShieldMessage>(P2POpCode.DamageShield);
            Register<AimedPointMessage>(P2POpCode.AimedPoint);
            Register<OnOffMessage>(P2POpCode.OnOff);
            Register<SentryGunSpawnMessage>(P2POpCode.SentryGunSpawn);
            Register<SentryGunStateMessage>(P2POpCode.SentryGunState);
            Register<SentryGunDestructionMessage>(P2POpCode.SentryGunDestruction);
            Register<FlyStateSyncMessage>(P2POpCode.FlyStateSync);
            Register<GrenadeSpawnMessage>(P2POpCode.GrenadeSpawn);
            Register<GrenadeSnapShotMessage>(P2POpCode.GrenadeSnapShot);
            Register<GrenadeExplosionMessage>(P2POpCode.GrenadeExplosion);
            Register<ObstructionSpawnMessage>(P2POpCode.ObstructionSpawn);
            Register<ObstructionDestroyMessage>(P2POpCode.ObstructionDestroy);
            Register<ObstructionDamageMessage>(P2POpCode.ObstructionDamage);
            Register<SyncObjectObstructionMessage>(P2POpCode.SyncObjectObstruction);
            Register<BlastObjectSyncMessage>(P2POpCode.BlastObjectSync);
            Register<BallSyncMessage>(P2POpCode.BallSync);
            Register<BallSnapShotMessage>(P2POpCode.BallSnapShot);
            Register<ArcadeFinMessage>(P2POpCode.ArcadeFin);
            Register<AttachArcadeItemMessage>(P2POpCode.AttachArcadeItem);
            Register<HPSyncMessage>(P2POpCode.HPSync);
            Register<DamageAIActorMessage>(P2POpCode.DamageAIActor);
            Register<ExposeClubMarkMessage>(P2POpCode.ExposeClubMark);
            Register<ReflectRateMessage>(P2POpCode.ReflectRate);
            Register<ConditionInfoMessage>(P2POpCode.ConditionInfo);
            Register<AbilityChangeSyncMessage>(P2POpCode.AbilityChangeSync);
            Register<HealHPMessage>(P2POpCode.HealHP);
            Register<WeaponInstallMessage>(P2POpCode.WeaponInstall);
            Register<PlayerRespawnInGameMessage>(P2POpCode.PlayerRespawnInGame);
            Register<Unk47Message>(P2POpCode.Unk47);
            Register<Unk48Message>(P2POpCode.Unk48);
            Register<Unk49Message>(P2POpCode.Unk49);
            Register<Unk50Message>(P2POpCode.Unk50);
            Register<Unk51Message>(P2POpCode.Unk51);
            Register<Unk52Message>(P2POpCode.Unk52);
            Register<EquippingItemSyncChangeMessage>(P2POpCode.EquippingItemSyncChange);
            Register<Unk54Message>(P2POpCode.Unk54);
            Register<Unk55Message>(P2POpCode.Unk55);
            Register<Unk56Message>(P2POpCode.Unk56);
            Register<Unk57Message>(P2POpCode.Unk57);
        }
    }
}
