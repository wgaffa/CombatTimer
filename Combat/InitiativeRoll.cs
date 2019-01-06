using DMTools.Die;
using DMTools.Die.Rollers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combat
{
    public enum InitiativeStatus
    {
        Active,
        Paused,
        Complete
    }

    public class InitiativeRoll : IEquatable<InitiativeRoll>, IComparable<InitiativeRoll>, INotifyPropertyChanged
    {
        private int _rolledInitiative;

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public InitiativeRoll(Character character, int rolledInitiative)
        {
            Character = character ?? throw new ArgumentNullException(nameof(character));
            RolledInitiative = rolledInitiative;
        }

        public InitiativeRoll(Character character, IDiceRoller diceRoller = null)
        {
            Character = character ?? throw new ArgumentNullException(nameof(character));

            RolledInitiative = RollNewInitiative(diceRoller);
        }

        private int RollNewInitiative(IDiceRoller diceRoller)
        {
            Dice dice = new Dice(20, diceRoller);

            return Character.Initiative + dice.Roll();
        }

        public Character Character { get; private set; }
        public int RolledInitiative {
            get => _rolledInitiative;
            set
            {
                if (_rolledInitiative == value) return;
                _rolledInitiative = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RolledInitiative)));
            }
        }
        public InitiativeStatus Status { get; set; }
        public int Priority { get; internal set; }

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
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (GetType() != obj.GetType()) return false;

            return Equals(obj as InitiativeRoll);
        }

        public bool Equals(InitiativeRoll other)
        {
            return other != null &&
                   EqualityComparer<Character>.Default.Equals(Character, other.Character);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 13;
                hashCode = hashCode * 23 + Character.GetHashCode();
                return hashCode;
            }

        }
    }
}
