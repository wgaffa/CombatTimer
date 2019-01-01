using Combat;
using Combat.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CombatTimer.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EncounterDetailsWindow _encounterWindow;
        private Point _mouseMoveStartPoint;

        public static RoutedCommand EncounterDetails = new RoutedCommand();

        public EncounterDetailsWindow EncounterWindow
        {
            get
            {
                if (_encounterWindow == null)
                    _encounterWindow = CreateEncounterWindow();

                return _encounterWindow;
            }

            private set => _encounterWindow = value;
        }
        public MainWindow(IEncounterRepository repository)
        {
            InitializeComponent();
            
            Encounter epicEncounter = repository.GetEncounter("Epic");
            
            DataContext = new InitiativeTrackerViewModel(epicEncounter);
        }

        public void OnInitiative_KeyDownCommand(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            UpdateBindingSource(sender as UIElement);
            e.Handled = true;
        }

        private void UpdateBindingSource(UIElement element)
        {
            if (element == null) return;

            if (!(element is TextBox textBox)) return;

            BindingExpression bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);

            bindingExpression?.UpdateSource();
        }

        private EncounterDetailsWindow CreateEncounterWindow()
        {
            EncounterViewModel encounter = new EncounterViewModel()
            {
                CurrentEncounter = ((InitiativeTrackerViewModel)DataContext).CurrentEncounter
            };
            EncounterDetailsWindow encounterWindow = new EncounterDetailsWindow(encounter);

            encounterWindow.Closed += (s, e) => EncounterWindow = null;

            return encounterWindow;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void EncounterDetails_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EncounterWindow.Show();
        }

        private void EncounterDetails_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
        private void ItemsControl_PreviewMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            _mouseMoveStartPoint = e.GetPosition(null);
        }

        private void ItemsControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource is TextBox) return;
            if (e.OriginalSource.GetType().FullName == "System.Windows.Controls.TextBoxView") return;

            Point currentMousePosition = e.GetPosition(null);
            Vector moveDiff = currentMousePosition - _mouseMoveStartPoint;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (
                Math.Abs(moveDiff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(moveDiff.Y) > SystemParameters.MinimumVerticalDragDistance
                ))
            {
                ItemsControl initiativeList = sender as ItemsControl;
                ContentPresenter initiativePresenter = FindAncestor<ContentPresenter>((DependencyObject)e.OriginalSource);

                InitiativeRoll initiativeRoll = (InitiativeRoll)initiativeList.ItemContainerGenerator.ItemFromContainer(initiativePresenter);
                DataObject dragData = new DataObject("initiativeRoll", initiativeRoll);

                DragDrop.DoDragDrop(initiativePresenter, dragData, DragDropEffects.Move);
            }
        }

        private T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (ReferenceEquals(typeof(T), current.GetType()))
                    return (T)current;

                current = VisualTreeHelper.GetParent(current);
            } while (current != null);

            return null;
        }
    }
}
