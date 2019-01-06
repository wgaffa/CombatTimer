﻿using Combat.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CombatTimer.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            JsonEncounterRepository encounterRepository = new JsonEncounterRepository(File.ReadAllText("sample-encounter.json"));

            new MainWindow(encounterRepository).Show();
        }
    }
}
