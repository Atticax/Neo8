using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("randomshop_color")]
    public class RandomShopColorEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public byte Color { get; set; }

        [Column]
        public int Probability { get; set; }

        [Column]
        public byte Grade { get; set; }
    }
}
