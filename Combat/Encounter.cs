using System;
using System.Collections.Generic;
using System.Linq;

namespace Combat
{
    public class Encounter : IEquatable<Encounter>
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

        public bool Equals(Encounter other)
        {
            if (other == null) return false;

            return Identifier == other.Identifier;
        }

        public override string ToString()
        {
            return Identifier;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (GetType() != obj.GetType()) return false;

            return Equals(obj as Encounter);
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }
    }
}
