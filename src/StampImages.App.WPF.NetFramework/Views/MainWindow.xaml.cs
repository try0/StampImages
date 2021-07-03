using System.Windows;
using System.Windows.Input;

namespace StampImages.App.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            MouseDown += (s, e) => ContainerGrid.Focus();
        }


    }
}
