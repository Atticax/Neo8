using Netsphere.Database.Game;

namespace Netsphere.Server.Game.Data
{
    public class RandomShopEffect
    {
        public string Group { get; set; }

        public long EffectId { get; set; }

        public int Effect { get; set; }

        public uint Probability { get; set; }

        public RandomShopGrade Grade { get; set; }

        public RandomShopEffect(RandomShopEffectEntity entity) // tut mir leid.
        {
            Group = entity.Name;
            EffectId = entity.Id;
            Effect = entity.Effect;
            Probability = (uint)entity.Probability;
            Grade = (RandomShopGrade)entity.Grade;
        }
    }
}
