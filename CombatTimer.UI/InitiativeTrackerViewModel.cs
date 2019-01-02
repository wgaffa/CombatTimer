using Combat;
using Combat.Repositories;
using CombatTimer.UI.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace CombatTimer.UI
{
    class InitiativeTrackerViewModel : INotifyPropertyChanged
    {
        private Encounter _encounter;
        public EncounterTimer EncounterTimer { get; private set; }

        public InitiativeTrackerViewModel(Encounter encounter)
        {
            _encounter = encounter ?? throw new ArgumentNullException(nameof(encounter));

            EncounterTimer = CreateNewCombat();
            
            NewCombatCommand = new DelegateCommand(OnNewCombatCommand);
            TurnCompleteCommand = new DelegateCommand(OnTurnCompleteCommand);
            DelayCommand = new DelegateCommand(OnDelayCommand);
            BeginCombatCommand = new DelegateCommand(OnTurnCompleteCommand, CanBeginCombat);
        }

        private EncounterTimer CreateNewCombat()
        {
            List<InitiativeRoll> initiativeRolls = new List<InitiativeRoll>();
            foreach (Character character in _encounter.Characters)
            {
                initiativeRolls.Add(new InitiativeRoll(character));
            }

            return new EncounterTimer(initiativeRolls);
        }

        #region Commands
        public DelegateCommand NewCombatCommand { get; private set; }
        public DelegateCommand TurnCompleteCommand { get; private set; }
        public DelegateCommand DelayCommand { get; private set; }
        public DelegateCommand BeginCombatCommand { get; private set; }

        private void OnNewCombatCommand(object obj)
        {
            EncounterTimer = CreateNewCombat();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncounterTimer)));
            BeginCombatCommand.InvokeCanExecuteChanged();
        }

        private void OnDelayCommand(object obj)
        {
            EncounterTimer.Delay();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncounterTimer)));
        }

        private void OnTurnCompleteCommand(object obj)
        {
            EncounterTimer.Next();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncounterTimer)));
            BeginCombatCommand.InvokeCanExecuteChanged();
        }

        private bool CanBeginCombat(object obj)
        {
            return EncounterTimer.CurrentInitiative == null;
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
