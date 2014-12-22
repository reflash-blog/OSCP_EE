using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Elysium;
using Framework.UI.Controls;
using OSProject.ViewModel;

namespace OSProject.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    { 
        public SettingsWindow()
        {
            InitializeComponent();
            this.DataContext = new SettingsViewModel();
        }
    }
}
