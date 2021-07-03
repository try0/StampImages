using StampImages.App.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StampImages.App.WPF.Views
{
    /// <summary>
    /// StampPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class StampPanel : UserControl
    {
        public StampPanel()
        {
            InitializeComponent();

            MainWindowViewModel.CurrentTabViewModel.Subscribe(val =>
            {
                DataContext = val;
            });
        }
    }
}
