using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combat
{
    public class RoundTimer
    {
        private Dictionary<Character, TimeSpan> _roundTimes = new Dictionary<Character, TimeSpan>();

        public IReadOnlyDictionary<Character, TimeSpan> RoundTimes => new ReadOnlyDictionary<Character, TimeSpan>(_roundTimes);

        public void Add(Character character, TimeSpan timeSpan)
        {
            if (character == null)
                throw new ArgumentNullException(nameof(character));

            if (timeSpan == null)
                throw new ArgumentNullException(nameof(timeSpan));

            _roundTimes.Add(character, timeSpan);
        }
    }
}
