using Netsphere.Database.Game;

namespace Netsphere.Server.Game.Data
{
    public class RandomShopColor
    {
        public string Group { get; set; }

        public byte Color { get; set; }

        public uint Probability { get; set; }

        public RandomShopGrade Grade { get; set; }

        public RandomShopColor(RandomShopColorEntity entity) //ugly sorry.
        {
            Group = entity.Name;
            Color = entity.Color;
            Probability = (uint)entity.Probability;
            Grade = (RandomShopGrade)entity.Grade;
        }
    }
}
