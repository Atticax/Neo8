namespace Netsphere.Common.Messaging
{
    public class LevelFromExperienceRequest : MessageWithGuid
    {
        public uint TotalExperience { get; set; }

        public LevelFromExperienceRequest()
        {
        }

        public LevelFromExperienceRequest(uint totalExperience)
        {
            TotalExperience = totalExperience;
        }
    }

    public class LevelFromExperienceResponse : MessageWithGuid
    {
        public int Level { get; set; }

        public LevelFromExperienceResponse()
        {
        }

        public LevelFromExperienceResponse(int level)
        {
            Level = level;
        }
    }
}
