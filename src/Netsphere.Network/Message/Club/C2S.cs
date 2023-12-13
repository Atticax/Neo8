using BlubLib.Serialization;
using Netsphere.Network.Data.Club;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Message.Club
{
    [BlubContract]
    public class ClubCreateReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public string Unk { get; set; }

        [BlubMember(1)]
        public string Name { get; set; }

        [BlubMember(2)]
        public string Description { get; set; }

        [BlubMember(3)]
        public ClubArea Area { get; set; }

        [BlubMember(4)]
        public ClubActivity Activity { get; set; }

        [BlubMember(5)]
        public string Question1 { get; set; }

        [BlubMember(6)]
        public string Question2 { get; set; }

        [BlubMember(7)]
        public string Question3 { get; set; }

        [BlubMember(8)]
        public string Question4 { get; set; }

        [BlubMember(9)]
        public string Question5 { get; set; }
    }

    [BlubContract]
    public class ClubCloseReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int ClubId { get; set; }
    }

    [BlubContract]
    public class ClubJoinReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }

        [BlubMember(1)]
        public string ClubName { get; set; }

        [BlubMember(2)]
        public string Answer1 { get; set; }

        [BlubMember(3)]
        public string Answer2 { get; set; }

        [BlubMember(4)]
        public string Answer3 { get; set; }

        [BlubMember(5)]
        public string Answer4 { get; set; }

        [BlubMember(6)]
        public string Answer5 { get; set; }
    }

    [BlubContract]
    public class ClubUnjoinReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubNameCheckReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public string Name { get; set; }
    }

    [BlubContract]
    public class ClubRestoreReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubAdminInviteReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class ClubAdminJoinCommandReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubCommand Command { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] AccountIds { get; set; }
    }

    [BlubContract]
    public class ClubAdminGradeChangeReqMessage : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public AdminGradeDto[] Grades { get; set; }
    }

    [BlubContract]
    public class ClubAdminNoticeChangeReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public string Notice { get; set; }
    }

    [BlubContract]
    public class ClubAdminInfoModifyReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubArea Area { get; set; }

        [BlubMember(1)]
        public ClubActivity Activity { get; set; }

        [BlubMember(2)]
        public string Description { get; set; }
    }

    [BlubContract]
    public class ClubAdminSubMasterReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubAdminSubMasterCancelReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class ClubAdminMasterChangeReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class ClubAdminJoinConditionModifyReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int JoinType { get; set; }

        [BlubMember(1)]
        public int RequiredLevel { get; set; }

        [BlubMember(2)]
        public string Question1 { get; set; }

        [BlubMember(3)]
        public string Question2 { get; set; }

        [BlubMember(4)]
        public string Question3 { get; set; }

        [BlubMember(5)]
        public string Question4 { get; set; }

        [BlubMember(7)]
        public string Question5 { get; set; }
    }

    [BlubContract]
    public class ClubAdminBoardModifyReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }
    }

    [BlubContract]
    public class ClubSearchReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubSearchType SearchType { get; set; }

        [BlubMember(1)]
        public string Query { get; set; }

        [BlubMember(2)]
        public int Page { get; set; }

        [BlubMember(3)]
        public ClubSearchSort SortBy { get; set; }

        [BlubMember(4)]
        public ClubSearchSortType SortType { get; set; }
    }

    [BlubContract]
    public class ClubInfoReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubJoinWaiterInfoReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubNewJoinMemberInfoReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubJoinConditionInfoReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubUnjoinerListReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubUnjoinSettingMemberListReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubGradeCountReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubStuffListReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubNewsInfoReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubBoardWriteReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public string Unk4 { get; set; }
    }

    [BlubContract]
    public class ClubBoardReadReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubBoardModifyReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public string Unk4 { get; set; }
    }

    [BlubContract]
    public class ClubBoardDeleteReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubBoardDeleteAllReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubBoardSearchNickReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class ClubBoardReadOtherClubReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubBoardReadMineReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubCreateReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public string Name { get; set; }

        [BlubMember(1)]
        public string Unk { get; set; }

        [BlubMember(2)]
        public string Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubCloseReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubJoinReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }

        [BlubMember(1)]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class ClubUnjoinReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubRestoreReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubClubInfoReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubSearchReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public string ClanName { get; set; }

        [BlubMember(2)]
        public int Unk2 { get; set; }

        [BlubMember(3)]
        public int Unk3 { get; set; }

        [BlubMember(4)]
        public byte Unk4 { get; set; }
    }

    [BlubContract]
    public class ClubEditURLReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public string Url { get; set; }
    }

    [BlubContract]
    public class ClubEditIntroduceReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public string Text { get; set; }
    }

    [BlubContract]
    public class ClubStuffListReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubRankListReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }
}
