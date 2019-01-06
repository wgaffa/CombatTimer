using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combat
{
    public class InitiativeTracker : INotifyPropertyChanged
    {
        public int CurrentRound { get; private set; } = 1;

        public InitiativeRoll CurrentInitiative { get; private set; }

        private List<InitiativeRoll> _initiatives;

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /// <summary>
        /// Initialize a new instance of the <see cref="InitiativeTracker"/> class.
        /// </summary>
        /// <param name="initiatives">A collection of initiative rolls.</param>
        public InitiativeTracker(IEnumerable<InitiativeRoll> initiatives)
        {
            if (initiatives == null)
                throw new ArgumentNullException(nameof(initiatives));

            if (initiatives.Count() < 2)
                throw new ArgumentException("atleast two combatants required", nameof(initiatives));

            _initiatives = initiatives.ToList();
            SortInitiatives();

            foreach (InitiativeRoll initiative in _initiatives)
            {
                initiative.PropertyChanged += Initiative_PropertyChanged;
            }
        }

        private void Initiative_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RolledInitiative") SortInitiatives();
        }

        public IReadOnlyList<InitiativeRoll> Initiatives => _initiatives.AsReadOnly();

        /// <summary>
        /// Start the next turn.
        /// </summary>
        /// <returns>The next <see cref="InitiativeRoll"/> to act.</returns>
        public virtual InitiativeRoll Next()
        {
            if (CurrentInitiative == null)
                return BeginSkirmish();

            End();
            InitiativeRoll nextInitiative = GetInitiativeOffset();

            bool newRound = nextInitiative == null;
            if (newRound)
            {
                nextInitiative = BeginNewRound();
            }

            if (nextInitiative.Status != InitiativeStatus.Active && nextInitiative.Status != InitiativeStatus.Complete)
                nextInitiative.Status = InitiativeStatus.Active;

            CurrentInitiative = nextInitiative;

            return nextInitiative;
        }

        public virtual InitiativeRoll Delay()
        {
            if (CurrentInitiative == null)
                throw new InvalidOperationException("skirmish not started, unable to delay");

            return PauseCurrentInitiative();
        }

        public virtual InitiativeRoll Resume(InitiativeRoll initiativeToResume)
        {
            initiativeToResume.Status = InitiativeStatus.Active;

            BumpInitiativeBeforeCurrent(initiativeToResume);

            CurrentInitiative = initiativeToResume;
            CurrentInitiative.Status = InitiativeStatus.Active;

            return initiativeToResume;
        }

        public void Move(InitiativeRoll source, InitiativeRoll target)
        {
            if (source == target) return;
            if (!_initiatives.Contains(source) || !_initiatives.Contains(target)) return;

            int nextInitiativeCount = target.RolledInitiative;
            int previousInitiativeCount = GetInitiativeOffset(-1, target)?.RolledInitiative ?? nextInitiativeCount + 20;

            int initiativeCountBetween = (nextInitiativeCount + previousInitiativeCount) / 2;
            source.RolledInitiative = initiativeCountBetween;

            SortInitiatives();
        }

        private InitiativeRoll PauseCurrentInitiative()
        {
            CurrentInitiative.Status = InitiativeStatus.Paused;

            InitiativeRoll nextInitiative = GetInitiativeOffset();

            CurrentInitiative = nextInitiative ?? BeginNewRound();

            return CurrentInitiative;
        }

        private InitiativeRoll GetInitiativeOffset(int offset = 1, InitiativeRoll target = null)
        {
            int index = _initiatives.IndexOf(target ?? CurrentInitiative);
            return _initiatives.ElementAtOrDefault(index + offset);
        }

        internal void BumpInitiativeBeforeCurrent(InitiativeRoll initiativeToResume)
        {
            int nextInitiative = CurrentInitiative.RolledInitiative;
            int previousInitiative = GetInitiativeOffset(-1)?.RolledInitiative ?? nextInitiative + 20;

            int initiativeBetween = (previousInitiative + nextInitiative) / 2;
            initiativeToResume.RolledInitiative = initiativeBetween;

            SortInitiatives();
        }

        private void SortInitiatives()
        {
            _initiatives = _initiatives
                .OrderByDescending(x => x.RolledInitiative)
                .ThenByDescending(x => x.Character.Initiative)
                .ToList();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Initiatives)));
        }

        private InitiativeRoll BeginNewRound()
        {
            InitiativeRoll nextInitiative = _initiatives.Max();
            foreach (InitiativeRoll initiative in _initiatives)
            {
                if (initiative.Status == InitiativeStatus.Complete)
                    initiative.Status = InitiativeStatus.Active;
            }

            CurrentRound++;
            return nextInitiative;
        }

        private void End()
        {
            Debug.Assert(CurrentInitiative != null);
            CurrentInitiative.Status = InitiativeStatus.Complete;
        }

        private InitiativeRoll BeginSkirmish()
        {
            SortInitiatives();
            CurrentInitiative = _initiatives.Max();
            return CurrentInitiative;
        }
    }
}
