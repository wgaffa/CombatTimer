using System;

namespace Combat
{
    public enum StatusType
    {
        Alive,
        Unconcious,
        Dead
    }

    public class Character : IEquatable<Character>
    {
        public Character(string name, int initiative = 0, StatusType status = StatusType.Alive)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("cannot be null or contain only whitespace", nameof(name));

            Name = name;
            Initiative = initiative;
        }

        public string Name { get; private set; }
        public int Initiative { get; private set; }
        public StatusType Status { get; set; }

        public bool Equals(Character other)
        {
            if (other == null)
                return false;

            return Name == other.Name
                && Initiative == other.Initiative;
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals(obj as Character);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 13;
                hashCode = (hashCode * 23) + Name.GetHashCode();
                hashCode = (hashCode * 23) + Initiative;

                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Initiative})";
        }
    }
}
