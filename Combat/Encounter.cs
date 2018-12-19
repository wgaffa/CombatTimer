using System;
using System.Collections.Generic;
using System.Linq;

namespace Combat
{
    public class Encounter
    {
        private HashSet<Character> _characters;

        public string Identifier { get; private set; }
        public IReadOnlyList<Character> Characters => _characters.ToList();
        
        public Encounter(string identifier, IEnumerable<Character> characters = null)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("cannot be null or contain only whitespace", nameof(identifier));

            Identifier = identifier;
            _characters = characters == null ? new HashSet<Character>() : new HashSet<Character>(characters);
        }
    }
}
