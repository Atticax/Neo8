using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;
using System;

namespace Netsphere.Server.Game.Data
{
    public class ShopEffectGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public uint PreviewEffect { get; set; }
        public IList<ShopEffect> Effects { get; set; }

        public ShopEffectGroup(ShopEffectGroupEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            PreviewEffect = entity.PreviewEffect;
            Effects = (IList<ShopEffect>)entity.ShopEffects.Select<ShopEffectEntity, ShopEffect>((Func<ShopEffectEntity, ShopEffect>)(x => new ShopEffect(x))).ToList<ShopEffect>();
        }

        public uint[] GetEffects() => Effects.Select<ShopEffect, uint>((Func<ShopEffect, uint>)(x => x.Effect)).ToArray<uint>();

        public ShopEffect GetEffect(int id)
        {
            return Effects.FirstOrDefault(x => x.Id == id);
        }

        public ShopEffect GetEffectByEffect(uint effect)
        {
            return Effects.FirstOrDefault(x => x.Effect == effect);
        }
    }
}
