using DMTools.Die.Rollers;
using System.Collections.Generic;
using System.Linq;

namespace CombatTimer.Tests
{
    internal class MockListRoller : IDiceRoller
    {
        private List<int> _rollValues;
        private int _index = -1;

        public MockListRoller(IEnumerable<int> rollValues)
        {
            _rollValues = rollValues.ToList();
        }

        public int RollDice(int sides)
        {
            _index = ++_index > _rollValues.Count - 1 ? 0 : _index;
            return _rollValues[_index];
        }
    }
}