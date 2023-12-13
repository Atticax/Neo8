using BlubLib.Serialization;

namespace Netsphere.Network.Data.GameRule
{
    [BlubContract]
    public class CaptainLifeDto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public float HP { get; set; }

        public CaptainLifeDto()
        {
        }

        public CaptainLifeDto(ulong accountId, float hp)
        {
            AccountId = accountId;
            HP = hp;
        }
    }
}
