namespace Netsphere.Common.Messaging
{
    public class ClanMemberUpdateMessage
    {
        public uint ClanId { get; set; }
        public ulong AccountId { get; set; }
        public ClubMemberPresenceState PresenceState { get; set; }
        public bool LoggedIn { get; set; }

        public ClanMemberUpdateMessage()
        {
        }

        public ClanMemberUpdateMessage(uint clanId, ulong accountId,
            ClubMemberPresenceState presenceState, bool loggedIn = false)
        {
            ClanId = clanId;
            AccountId = accountId;
            PresenceState = presenceState;
            LoggedIn = loggedIn;
        }
    }
}
