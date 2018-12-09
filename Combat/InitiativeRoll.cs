﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combat
{
    public enum ActionTakenType
    {
        None,
        TurnComplete
    }

    public class InitiativeRoll : IEquatable<InitiativeRoll>, IComparable<InitiativeRoll>
    {
        public InitiativeRoll(Character character, int rolledInitiative)
        {
            Character = character ?? throw new ArgumentNullException(nameof(character));
            RolledInitiative = rolledInitiative;
        }

        public Character Character { get; private set; }
        public int RolledInitiative { get; private set; }
        public ActionTakenType ActionTaken { get; set; }

        public int CompareTo(InitiativeRoll other)
        {
            if (RolledInitiative == other.RolledInitiative)
                return 0;

            if (RolledInitiative < other.RolledInitiative)
                return -1;

            return 1;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as InitiativeRoll);
        }

        public bool Equals(InitiativeRoll other)
        {
            return other != null &&
                   EqualityComparer<Character>.Default.Equals(Character, other.Character) &&
                   RolledInitiative == other.RolledInitiative;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 13;
                hashCode = hashCode * 23 + Character.GetHashCode();
                hashCode = hashCode * 23 + RolledInitiative;
                return hashCode;
            }
            
        }
    }
}