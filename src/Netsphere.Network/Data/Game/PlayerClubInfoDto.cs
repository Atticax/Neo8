using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class PlayerClubInfoDto
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }

        [BlubMember(6)]
        public string ClubName { get; set; }

        [BlubMember(7)]
        public string ClubType { get; set; }
    }
}
