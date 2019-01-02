using Combat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatTimer.UI
{
    class EncounterViewModel : INotifyPropertyChanged
    {
        private Encounter _currentEncounter;
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public Encounter CurrentEncounter
        {
            get => _currentEncounter;
            set
            {
                if (_currentEncounter == value) return;

                _currentEncounter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentEncounter)));
            }
        }
    }
}
