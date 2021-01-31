using System;
using System.Windows;
using System.Windows.Input;

namespace SocialSimulation
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

        private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
        }
    }

}
