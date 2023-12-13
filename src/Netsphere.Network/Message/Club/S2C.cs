using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.Club;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Message.Club
{
    [BlubContract]
    public class ClubCreateAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubCreateResult Result { get; set; }

        public ClubCreateAckMessage()
        {
        }

        public ClubCreateAckMessage(ClubCreateResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class ClubCloseAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubCloseResult Result { get; set; }

        public ClubCloseAckMessage()
        {
        }

        public ClubCloseAckMessage(ClubCloseResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class ClubJoinAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubJoinResult Result { get; set; }

        public ClubJoinAckMessage()
        {
        }

        public ClubJoinAckMessage(ClubJoinResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class ClubUnjoinAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubLeaveResult Result { get; set; }

        public ClubUnjoinAckMessage()
        {
        }

        public ClubUnjoinAckMessage(ClubLeaveResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class ClubNameCheckAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubNameCheckResult Result { get; set; }

        public ClubNameCheckAckMessage()
        {
        }

        public ClubNameCheckAckMessage(ClubNameCheckResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class ClubRestoreAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubAdminInviteAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubAdminInviteResult Result { get; set; }

        public ClubAdminInviteAckMessage()
        {
        }

        public ClubAdminInviteAckMessage(ClubAdminInviteResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class ClubAdminJoinCommandAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubCommandResult Result { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] AccountIds { get; set; }

        public ClubAdminJoinCommandAckMessage()
        {
            AccountIds = Array.Empty<ulong>();
        }

        public ClubAdminJoinCommandAckMessage(ClubCommandResult result)
            : this()
        {
            Result = result;
        }

        public ClubAdminJoinCommandAckMessage(ClubCommandResult result, ulong[] accountIds)
        {
            Result = result;
            AccountIds = accountIds;
        }
    }

    [BlubContract]
    public class ClubAdminGradeChangeAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubAdminChangeRoleResult Result { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] AccountIdsChanged { get; set; }

        public ClubAdminGradeChangeAckMessage()
        {
        }

        public ClubAdminGradeChangeAckMessage(ClubAdminChangeRoleResult result)
            : this(result, Array.Empty<ulong>())
        {
        }

        public ClubAdminGradeChangeAckMessage(ClubAdminChangeRoleResult result, ulong[] accountIdsChanged)
        {
            Result = result;
            AccountIdsChanged = accountIdsChanged;
        }
    }

    [BlubContract]
    public class ClubAdminNoticeChangeAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubNoticeChangeResult Result { get; set; }

        public ClubAdminNoticeChangeAckMessage()
        {
        }

        public ClubAdminNoticeChangeAckMessage(ClubNoticeChangeResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class ClubAdminInfoModifyAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubAdminInfoModifyResult Result { get; set; }

        public ClubAdminInfoModifyAckMessage()
        {
        }

        public ClubAdminInfoModifyAckMessage(ClubAdminInfoModifyResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class ClubAdminSubMasterAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubAdminSubMasterCancelAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubAdminMasterChangeAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubAdminJoinConditionModifyAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubAdminJoinConditionModifyResult Result { get; set; }

        public ClubAdminJoinConditionModifyAckMessage()
        {
        }

        public ClubAdminJoinConditionModifyAckMessage(ClubAdminJoinConditionModifyResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class ClubAdminBoardModifyAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubSearchAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int ResultCount { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ClubSearchResultDto[] Clubs { get; set; }

        public ClubSearchAckMessage()
        {
            Clubs = Array.Empty<ClubSearchResultDto>();
        }

        public ClubSearchAckMessage(ClubSearchResultDto[] clubs)
        {
            ResultCount = clubs.Length;
            Clubs = clubs;
        }
    }

    [BlubContract]
    public class ClubInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClanId { get; set; }

        [BlubMember(1)]
        public string ClanIcon { get; set; }

        [BlubMember(2)]
        public string ClanName { get; set; }

        [BlubMember(3)]
        public int MemberCount { get; set; }

        [BlubMember(4)]
        public string OwnerName { get; set; }

        [BlubMember(5)]
        [BlubSerializer(typeof(ClubCreationDateSerializer))]
        public DateTimeOffset CreationDate { get; set; }

        [BlubMember(6)]
        public ClubArea Area { get; set; }

        [BlubMember(7)]
        public ClubActivity Activity { get; set; }

        [BlubMember(8)]
        public int Wins { get; set; }

        [BlubMember(9)]
        public int Losses { get; set; }

        [BlubMember(10)]
        public ClubClass Class { get; set; }

        [BlubMember(11)]
        public int Unk12 { get; set; }

        [BlubMember(12)]
        public string Description { get; set; }

        [BlubMember(13)]
        public string Announcement { get; set; }

        [BlubMember(14)]
        public int Unk15 { get; set; }

        [BlubMember(15)]
        public int Unk16 { get; set; }

        [BlubMember(16)]
        public int Unk17 { get; set; }

        [BlubMember(17)]
        public int Unk18 { get; set; }
    }

    [BlubContract]
    public class ClubJoinWaiterInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public JoinWaiterInfoDto[] Waiters { get; set; }
    }

    [BlubContract]
    public class ClubNewJoinMemberInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public NewMemberInfoDto[] NewMembers { get; set; }

        public ClubNewJoinMemberInfoAckMessage()
        {
            NewMembers = Array.Empty<NewMemberInfoDto>();
        }

        public ClubNewJoinMemberInfoAckMessage(NewMemberInfoDto[] newMembers)
        {
            NewMembers = newMembers;
        }
    }

    [BlubContract]
    public class ClubJoinConditionInfoAckMessage : IClubMessage
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

        [BlubMember(6)]
        public string Question5 { get; set; }
    }

    [BlubContract]
    public class ClubUnjoinerListAckMessage : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public MemberLeftDto[] Members { get; set; }

        public ClubUnjoinerListAckMessage()
        {
            Members = Array.Empty<MemberLeftDto>();
        }

        public ClubUnjoinerListAckMessage(MemberLeftDto[] members)
        {
            Members = members;
        }
    }

    [BlubContract]
    public class ClubUnjoinSettingMemberListAckMessage : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public UnjoinSettingMemberDto[] Unk { get; set; }
    }

    [BlubContract]
    public class ClubGradeCountAckMessage : IClubMessage
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
    public class ClubStuffListAckMessage : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public Netsphere.Network.Data.Chat.ClubMemberDto[] Members { get; set; }
    }

    [BlubContract]
    public class ClubNewsInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public NewsInfoDto[] News { get; set; }
    }

    [BlubContract]
    public class ClubMyInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClanId { get; set; }

        [BlubMember(1)]
        public string ClanIcon { get; set; }

        [BlubMember(2)]
        public string ClanName { get; set; }

        [BlubMember(3)]
        public ClubMemberState State { get; set; }

        [BlubMember(4)]
        public int Unk5 { get; set; }

        [BlubMember(5)]
        public ClubRole Role { get; set; }

        [BlubMember(6)]
        public int Unk7 { get; set; }

        [BlubMember(7)]
        public int Unk8 { get; set; }

        [BlubMember(8)]
        public int LeaguePointWin { get; set; }

        [BlubMember(9)]
        public int LeaguePointLose { get; set; }

        [BlubMember(10)]
        public int ContributionPointWin { get; set; }

        [BlubMember(11)]
        public int ContributionPointLose { get; set; }

        [BlubMember(12)]
        public bool Unk13 { get; set; }
    }

    [BlubContract]
    public class ClubBoardWriteAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubBoardReadAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public BoardInfoDto[] Unk2 { get; set; }

        public ClubBoardReadAckMessage() => Unk2 = Array.Empty<BoardInfoDto>();
    }

    [BlubContract]
    public class ClubBoardModifyAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubBoardDeleteAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubBoardDeleteAllAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubBoardReadFailedAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubCreateAck2Message : IClubMessage
    {
        [BlubMember(0)]
        public ClubCreateResult Result { get; set; }

        [BlubMember(1)]
        public ClubCreateResult Unk1 { get; set; }

        public ClubCreateAck2Message()
        {
        }

        public ClubCreateAck2Message(ClubCreateResult result)
        {
            Result = result;
            Unk1 = result;
        }
    }

    [BlubContract]
    public class ClubCloseAck2Message : IClubMessage
    {
        [BlubMember(0)]
        public ClubCloseResult Result { get; set; }

        public ClubCloseAck2Message()
        {
        }

        public ClubCloseAck2Message(ClubCloseResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class ClubJoinAck2Message : IClubMessage
    {
        [BlubMember(0)]
        public ClubJoinResult Result { get; set; }

        public ClubJoinAck2Message()
        {
        }

        public ClubJoinAck2Message(ClubJoinResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class ClubUnjoinAck2Message : IClubMessage
    {
        [BlubMember(0)]
        public ClubLeaveResult Result { get; set; }

        public ClubUnjoinAck2Message()
        {
        }

        public ClubUnjoinAck2Message(ClubLeaveResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class ClubRestoreAck2Message : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubClubInfoAck2Message : IClubMessage
    {
        [BlubMember(0)]
        public int ClanId { get; set; }

        [BlubMember(1)]
        public string ClanName { get; set; }

        [BlubMember(2)]
        public string ClanIcon { get; set; }

        [BlubMember(3)]
        public string Unk1 { get; set; }

        [BlubMember(4)]
        public ulong Unk2 { get; set; }

        [BlubMember(5)]
        public int Unk3 { get; set; }

        [BlubMember(6)]
        public string OwnerName { get; set; }

        [BlubMember(7)]
        public int Unk4 { get; set; }

        [BlubMember(8)]
        public int PlayersCount { get; set; }

        [BlubMember(9)]
        public string Unk5 { get; set; }

        [BlubMember(10)]
        [BlubSerializer(typeof(ClubCreationDateSerializer))]
        public DateTimeOffset CreationDate { get; set; }

        [BlubMember(11)]
        public string Unk7 { get; set; }

        [BlubMember(12)]
        public string Unk8 { get; set; }

        [BlubMember(13)]
        public int Unk9 { get; set; }

        [BlubMember(14)]
        public int Unk10 { get; set; }

        [BlubMember(15)]
        public int Unk11 { get; set; }

        [BlubMember(16)]
        public int Unk12 { get; set; }

        [BlubMember(17)]
        public int ClubPoints { get; set; }

        [BlubMember(18)]
        public int Unk13 { get; set; }

        [BlubMember(19)]
        public int ClubRankWins { get; set; }

        [BlubMember(20)]
        public int ClubRankDefeats { get; set; }

        [BlubMember(21)]
        public int LadderPoints { get; set; }

        [BlubMember(22)]
        public short Unk14 { get; set; }

        [BlubMember(23)]
        public int Unk15 { get; set; }
    }

    [BlubContract]
    public class ClubSearchAck2Message : IClubMessage
    {
        [BlubMember(0)]
        public int ResultCount { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ClubInfo2Dto[] Clubs { get; set; }

        public ClubSearchAck2Message() => Clubs = Array.Empty<ClubInfo2Dto>();

        public ClubSearchAck2Message(int resultCount, ClubInfo2Dto[] clubs)
        {
            ResultCount = resultCount;
            Clubs = clubs;
        }

    }

    [BlubContract]
    public class ClubEditURLAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubEditIntroduceAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubStuffListAck2Message : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public Netsphere.Network.Data.Chat.ClubMember2Dto[] Members { get; set; }
    }

    [BlubContract]
    public class ClubRankListAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int TotalClubs { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ClubInfo2Dto[] Clubs { get; set; }

        public ClubRankListAckMessage() => Clubs = Array.Empty<ClubInfo2Dto>();

        public ClubRankListAckMessage(int totalClubs, ClubInfo2Dto[] clubs)
        {
            TotalClubs = totalClubs;
            Clubs = clubs;
        }
    }

    [BlubContract]
    public class ClubMatchNoticeMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }
}
