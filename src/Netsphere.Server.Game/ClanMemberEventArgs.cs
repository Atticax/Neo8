using System;

namespace Netsphere.Server.Game
{
    public class ClanMemberEventArgs : EventArgs
    {
        public ClanMember Member { get; }

        public ClanMemberEventArgs(ClanMember member)
        {
            Member = member;
        }
    }
}
