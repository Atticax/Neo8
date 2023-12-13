using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Netsphere.Server.Game
{
    public class PlayerItemEffectCollection : IEnumerable<uint>
    {
        private readonly List<uint> _effects = new List<uint>();
        private readonly PlayerItem _item;

        public PlayerItemEffectCollection(PlayerItem item, IEnumerable<uint> effects = null)
        {
            _item = item;
            if (effects != null)
                _effects.AddRange(effects);
        }

        public int Count => _effects.Count;

        public void Add(uint effect)
        {
            _effects.Add(effect);
            _item.SetDirtyState(true);
        }

        public bool Remove(uint effect)
        {
            var result = _effects.Remove(effect);
            if (result)
                _item.SetDirtyState(true);

            return result;
        }

        public void ResetEnchant()
        {
            foreach (uint num in _effects.Where(x => x > int.MaxValue).ToList())
                _effects.Remove(num);
            _item.SetDirtyState(true);
        }

        public IEnumerator<uint> GetEnumerator()
        {
            return _effects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
