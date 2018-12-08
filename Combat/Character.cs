using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combat
{
    public enum StatusType
    {
        Alive,
        Unconcious,
        Dead
    }

    public class Character
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
    }
}
