using System;
using System.Collections.Generic;
using System.Linq;

namespace Combat
{
    public class Encounter
    {
        private HashSet<Character> _characters = new HashSet<Character>();

        public string Identifier { get; private set; }
        public IReadOnlyList<Character> Characters
        {
            get => _characters.ToList();
            private set => _characters = new HashSet<Character>(value);
        }
        
        public Encounter(string identifier, IEnumerable<Character> characters = null)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("cannot be null or contain only whitespace", nameof(identifier));

            Identifier = identifier;
            Characters = characters?.ToList() ?? new List<Character>();
        }
    }
}
