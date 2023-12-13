namespace Netsphere.Common.Messaging
{
    public class PlayerUpdateMessage
    {
        public ulong AccountId { get; set; }
        public uint TotalExperience { get; set; }
        public int Level { get; set; }
        public uint RoomId { get; set; }
        public uint Nametag { get; set; }
        public TeamId TeamId { get; set; }

        public PlayerUpdateMessage()
        {
        }

        public PlayerUpdateMessage(ulong accountId, uint totalExperience, int level, uint roomId, uint nametag, TeamId teamId)
        {
            AccountId = accountId;
            TotalExperience = totalExperience;
            Level = level;
            RoomId = roomId;
            Nametag = nametag;
            TeamId = teamId;
        }
    }
}
