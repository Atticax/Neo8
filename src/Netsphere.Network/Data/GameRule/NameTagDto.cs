using BlubLib.Serialization;

namespace Netsphere.Network.Data.GameRule
{
    [BlubContract]
    public class NameTagDto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public uint NameTag { get; set; }

        public NameTagDto()
        {
        }

        public NameTagDto(ulong accountId, uint nameTag)
        {
            NameTag = nameTag;
        }
    }
}
