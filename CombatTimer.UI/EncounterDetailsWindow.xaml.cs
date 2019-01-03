using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace CombatTimer.UI
{
    /// <summary>
    /// Interaction logic for EncounterDetailsWindow.xaml
    /// </summary>
    public partial class EncounterDetailsWindow : Window
    {
        public EncounterDetailsWindow(EncounterViewModel encounter)
        {
            InitializeComponent();
            DataContext = encounter;
        }
    }
}
